// <copyright file="RepositoryWithVersionControl.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Fursvp.Domain;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Provides caching and version control when persisting state changes on new, updated or removed documents using a decorated instance <see cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public class RepositoryWithVersionControl<T> : IRepository<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWithVersionControl{T}"/> class.
        /// </summary>
        /// <param name="decorated">The instance of <see cref="IRepository{T}"/> to decorate.</param>
        /// <param name="memoryCache">The instance of <see cref="IMapper"/> for deep copies.</param>
        /// <param name="mapper">The instance of <see cref="IMemoryCache"/> for caching.</param>
        public RepositoryWithVersionControl(IRepository<T> decorated, IMemoryCache memoryCache, IMapper mapper)
        {
            this.Decorated = decorated;
            this.MemoryCache = memoryCache;
            this.Mapper = mapper;
        }

        private IRepository<T> Decorated { get; }

        private IMemoryCache MemoryCache { get; }

        private IMapper Mapper { get; }

        private string IndexCacheKey => $"{typeof(RepositoryWithVersionControl<T>).Name}<{typeof(T).Name}>.Index";

        /// <summary>
        /// Permanently removes an existing document representing <see ref="T" /> from the repository by exposing the decorated method, and from the cache.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Delete(Guid guid)
        {
            await this.Decorated.Delete(guid);
            this.MemoryCache.Remove(guid);
        }

        /// <summary>
        /// Gets a result set containing all documents for the entity type from the cache, or via the decorated method if not yet cached.
        /// If not already cached, then an index is built and the index and all entities retrieved are cached.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> against which further filtering can be applied on the result set.</returns>
        public async Task<IQueryable<T>> GetAll()
        {
            if (this.MemoryCache.TryGetValue(this.IndexCacheKey, out List<Guid> guids))
            {
                return guids.Select(g =>
                {
                    var entity = this.MemoryCache.Get<T>(g);
                    return this.Mapper.Map<T, T>(entity);
                }).AsQueryable();
            }

            var allEntities = (await this.Decorated.GetAll()).ToList();
            DateTime expiration = DateTime.Now.AddMinutes(5); // TODO - put magic number into config

            this.MemoryCache.Set(this.IndexCacheKey, allEntities.Select(e => e.Id).ToList(), expiration);

            foreach (var e in allEntities)
            {
                this.CacheCopyOfEntity(e, expiration);
            }

            return allEntities.AsQueryable();
        }

        /// <summary>
        /// Gets a single instance of T matching the given Id from the cache, or via the decorated method if not already cached.
        /// If not already cached, the entity retrieved is cached.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        public async Task<T> GetById(Guid guid)
        {
            var entity = await this.MemoryCache.GetOrCreateAsync(guid, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // TODO - put magic number into config
                return await this.Decorated.GetById(guid);
            });

            return this.Mapper.Map<T, T>(entity);
        }

        /// <summary>
        /// Searches for a newer version of an entity in the database if it exists and returns it if found.
        /// If found, the newer version is saved to the cache.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, default(T).</returns>
        public async Task<T> GetNewerVersionIfExists(Guid guid, int version)
        {
            var entity = await this.Decorated.GetNewerVersionIfExists(guid, version);

            if (entity != null)
            {
                this.CacheCopyOfEntity(entity);
            }

            return this.Mapper.Map<T, T>(entity);
        }

        /// <summary>
        /// Persists to the repository and to the cache a new document representing the entity by exposing the decorated method.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Insert(T entity)
        {
            await this.Decorated.Insert(entity);
            this.CacheCopyOfEntity(entity);
        }

        /// <summary>
        /// Updates the entity's Version property and, if the versions do not conflict, overwrites the existing document in the repository and in the cache.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Update(T entity)
        {
            // TODO - it would be nice if this could be made into an atomic operation against Firestore.
            var entityInDb = await this.GetNewerVersionIfExists(entity.Id, entity.Version);

            if (entityInDb != null && entityInDb.Version != entity.Version)
            {
                throw new VersionControlException<T>("Conflict detected.");
            }

            entity.Version++;

            await this.Decorated.Update(entity);
            this.CacheCopyOfEntity(entity);
        }

        private void CacheCopyOfEntity(T entity)
        {
            var copy = this.Mapper.Map<T, T>(entity);
            this.MemoryCache.Set(entity.Id, copy, TimeSpan.FromMinutes(5)); // TODO - put magic number into config
        }

        private void CacheCopyOfEntity(T entity, DateTime expiration)
        {
            var copy = this.Mapper.Map<T, T>(entity);
            this.MemoryCache.Set(entity.Id, copy, expiration);
        }
    }
}
