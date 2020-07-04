// <copyright file="RepositoryWithVersionControl.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Domain;

    /// <summary>
    /// Provides version control when persisting state changes on new, updated or removed documents using a decorated instance <see cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public class RepositoryWithVersionControl<T> : IRepository<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWithVersionControl{T}"/> class.
        /// </summary>
        /// <param name="decorated">The instance of <see cref="IRepository{T}"/> to decorate.</param>
        public RepositoryWithVersionControl(IRepository<T> decorated)
        {
            this.Decorated = decorated;
        }

        private IRepository<T> Decorated { get; }

        /// <summary>
        /// Permanently removes an existing document representing <see ref="T" /> from the repository by exposing the decorated method.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Delete(Guid guid) => await this.Decorated.Delete(guid);

        /// <summary>
        /// Gets a result set containing all documents for the entity type by exposing the decorated method.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> against which further filtering can be applied on the result set.</returns>
        public async Task<IQueryable<T>> GetAll() => await this.Decorated.GetAll();

        /// <summary>
        /// Gets a single instance of T matching the given Id by exposing the decorated method.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        public async Task<T> GetById(Guid guid) => await this.Decorated.GetById(guid);

        /// <summary>
        /// Persists a new document representing the entity to the repository by exposing the decorated method.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Insert(T entity) => await this.Decorated.Insert(entity);

        /// <summary>
        /// Updates the entity's Version property and overwrites the existing document in the repository if the versions do not conflict.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Update(T entity)
        {
            var oldEntity = await this.Decorated.GetById(entity.Id);

            if (oldEntity.Version != entity.Version)
            {
                throw new VersionControlException<T>("Conflict detected.");
            }

            entity.Version++;

            await this.Decorated.Update(entity);
        }
    }
}
