// <copyright file="RepositoryWithAuthorization.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.RepositoryDecorators
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Data;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization.WriteAuthorization;
    using Fursvp.Domain.Validation;

    /// <summary>
    /// Authorizes state changes by a user on new, updated or removed documents using a decorated instance <see cref="IRepositoryWrite{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public class RepositoryWithAuthorization<T> : IRepositoryWrite<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWithAuthorization{T}"/> class.
        /// </summary>
        /// <param name="decorated">The instance of <see cref="IRepositoryWrite{T}"/> to decorate.</param>
        /// <param name="repositoryRead">The instance of <see cref="IRepositoryRead{T}"/> for read operations.</param>
        /// <param name="authorize">The instance of <see cref="IWriteAuthorize{T}"/> to perform authorization.</param>
        public RepositoryWithAuthorization(IRepositoryWrite<T> decorated, IRepositoryRead<T> repositoryRead, IWriteAuthorize<T> authorize)
        {
            this.Decorated = decorated;
            this.RepositoryRead = repositoryRead;
            this.Authorize = authorize;
        }

        private IRepositoryWrite<T> Decorated { get; }

        private IRepositoryRead<T> RepositoryRead { get; }

        private IWriteAuthorize<T> Authorize { get; }

        /// <summary>
        /// Persists a new document representing the entity to the repository if the user is authorized.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Insert(T entity)
        {
            this.Authorize.WriteAuthorize(default, entity);

            await this.Decorated.Insert(entity);
        }

        /// <summary>
        /// Overwrites an existing document representing the entity to the repository if the user is authorized.
        /// </summary>
        /// <param name="updatedEntity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Update(T updatedEntity)
        {
            var oldEntity = await this.RepositoryRead.GetById(updatedEntity.Id);

            if (oldEntity == null)
            {
                throw new ValidationException<T>("Must provide a valid id");
            }

            this.Authorize.WriteAuthorize(oldEntity, updatedEntity);

            await this.Decorated.Update(updatedEntity);
        }

        /// <summary>
        /// Permanently removes an existing document representing <see ref="T" /> from the repository if the user is authorized.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Delete(Guid guid)
        {
            var entity = await this.RepositoryRead.GetById(guid);

            this.Authorize.WriteAuthorize(entity, default);

            await this.Decorated.Delete(guid);
        }
    }
}
