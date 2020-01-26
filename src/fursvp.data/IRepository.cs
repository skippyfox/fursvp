// <copyright file="IRepository.cs" company="skippyfox">
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
    /// Provides "CRUD" persistence logic for a domain entity.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public interface IRepository<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Gets a result set containing all documents for the entity type.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> against which further filtering can be applied on the result set.</returns>
        Task<IQueryable<T>> GetAll();

        /// <summary>
        /// Gets a single instance of T matching the given Id.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        Task<T> GetById(Guid guid);

        /// <summary>
        /// Persists a new document representing the entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        Task Insert(T entity);

        /// <summary>
        /// Overwrites an existing document representing the entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        Task Update(T entity);

        /// <summary>
        /// Permanently removes an existing document representing the entity from the repository.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        Task Delete(Guid guid);
    }
}
