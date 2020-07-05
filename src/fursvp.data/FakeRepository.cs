// <copyright file="FakeRepository.cs" company="skippyfox">
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

    /// <summary>
    /// An in-memory repository store for an entity type usable for testing.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity{TEntity}"/> type.</typeparam>
    public class FakeRepository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRepository{TEntity}"/> class for debugging and testing.
        /// </summary>
        /// <param name="mapper">The instance of <see cref="IMapper" /> for making deep copies.</param>
        public FakeRepository(IMapper mapper)
        {
            Mapper = mapper;
        }

        private IMapper Mapper { get; }

        private List<TEntity> Entities { get; } = new List<TEntity>();

        /// <summary>
        /// Permanently removes an existing document representing TEntity from memory.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{TEntity}"/>.</returns>
        public Task Delete(Guid guid)
        {
            Entities.RemoveAll(e => e.Id == guid);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a result set from memory containing all documents for the entity type.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> against which further filtering can be applied on the result set.</returns>
        public Task<IQueryable<TEntity>> GetAll()
        {
            return Task.FromResult(Entities.Select(DeepCopy).AsQueryable());
        }

        /// <summary>
        /// Gets a single instance of T from memory matching the given Id.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{TEntity}"/> containing the entity if found, otherwise null.</returns>
        public Task<TEntity> GetById(Guid guid)
        {
            return Task.FromResult(DeepCopy(Entities.FirstOrDefault(e => e.Id == guid)));
        }

        /// <summary>
        /// Persists a new document representing TEntity to memory.
        /// </summary>
        /// <param name="entity">The entity to persist to memory.</param>
        /// <returns>An asynchronous <see cref="Task{TEntity}"/>.</returns>
        public Task Insert(TEntity entity)
        {
            Entities.Add(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Overwrites an existing document representing <see ref="TEntity" /> in memory.
        /// </summary>
        /// <param name="entity">The entity to persist to memory.</param>
        /// <returns>An asynchronous <see cref="Task{TEntity}"/>.</returns>
        public Task Update(TEntity entity)
        {
            Entities.RemoveAll(e => e.Id == entity.Id);
            Insert(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches for a newer version of <see ref="TEntity" /> in memory if it exists and returns it if found.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, null.</returns>
        public async Task<TEntity> GetNewerVersionIfExists(Guid guid, int version)
        {
            var @entityInMemory = await GetById(guid).ConfigureAwait(false);
            if (@entityInMemory?.Version > version)
            {
                return @entityInMemory;
            }

            return default(TEntity);
        }

        private TEntity DeepCopy(TEntity entity) => Mapper.Map<TEntity, TEntity>(entity);
    }
}
