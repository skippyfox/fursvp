// <copyright file="IRepositoryWrite.cs" company="skippyfox">
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
    /// Provides create/update/delete persistence logic for a domain entity.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public interface IRepositoryWrite<T>
        where T : IEntity<T>
    {
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
