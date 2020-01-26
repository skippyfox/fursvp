﻿// <copyright file="RepositoryWithAuthorization.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Domain.Validation;

    /// <summary>
    /// Authorizes state changes by a user on new, updated or removed documents using a decorated instance <see cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public class RepositoryWithAuthorization<T> : IRepository<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWithAuthorization{T}"/> class.
        /// </summary>
        /// <param name="decorated">The instance of <see cref="IRepository{T}"/> to decorate.</param>
        /// <param name="authorize">The instance of <see cref="IAuthorize{T}"/> to perform authorization.</param>
        public RepositoryWithAuthorization(IRepository<T> decorated, IAuthorize<T> authorize)
        {
            this.Decorated = decorated;
            this.Authorize = authorize;
        }

        private IRepository<T> Decorated { get; }

        private IAuthorize<T> Authorize { get; }

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
        /// Persists a new document representing the entity to the repository if the user is authorized.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Insert(T entity)
        {
            // TODO: Provide actor
            this.Authorize.Authorize(string.Empty, default, entity);

            await this.Decorated.Insert(entity);
        }

        /// <summary>
        /// Overwrites an existing document representing the entity to the repository if the user is authorized.
        /// </summary>
        /// <param name="updatedEntity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Update(T updatedEntity)
        {
            var oldEntity = await this.Decorated.GetById(updatedEntity.Id);

            if (oldEntity == null)
            {
                throw new ValidationException<T>("Must provide a valid id");
            }

            // TODO: Provide actor
            this.Authorize.Authorize(string.Empty, oldEntity, updatedEntity);

            await this.Decorated.Update(updatedEntity);
        }

        /// <summary>
        /// Permanently removes an existing document representing <see ref="T" /> from the repository if the user is authorized.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Delete(Guid guid)
        {
            var entity = await this.Decorated.GetById(guid);

            // TODO: Provide actor
            this.Authorize.Authorize(string.Empty, entity, default);

            await this.Decorated.Delete(guid);
        }
    }
}