// <copyright file="IRepositoryRead.cs" company="skippyfox">
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
    /// Provides logic to read from a data repository for a domain entity.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public interface IRepositoryRead<T>
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
        /// Searches for a newer version of an entity in the database if it exists and returns it if found.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, default(T).</returns>
        Task<T> GetNewerVersionIfExists(Guid guid, int version);
    }
}
