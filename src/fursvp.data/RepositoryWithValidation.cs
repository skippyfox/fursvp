// <copyright file="RepositoryWithValidation.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Domain;
    using Fursvp.Domain.Validation;

    /// <summary>
    /// Validates state changes on new, updated or removed documents using a decorated instance <see cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public class RepositoryWithValidation<T> : IRepository<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWithValidation{T}"/> class.
        /// </summary>
        /// <param name="decorated">The instance of <see cref="IRepository{T}"/> to decorate.</param>
        /// <param name="validator">The instance of <see cref="IValidate{T}"/> to perform validation.</param>
        public RepositoryWithValidation(IRepository<T> decorated, IValidate<T> validator)
        {
            this.Decorated = decorated;
            this.Validator = validator;
        }

        private IRepository<T> Decorated { get; }

        private IValidate<T> Validator { get; }

        /// <summary>
        /// Gets a result set containing all documents for the entity type by exposing the decorated method.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> against which further filtering can be applied on the result set.</returns>
        public Task<IQueryable<T>> GetAll() => this.Decorated.GetAll();

        /// <summary>
        /// Gets a single instance of T matching the given Id by exposing the decorated method.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        public Task<T> GetById(Guid guid) => this.Decorated.GetById(guid);

        /// <summary>
        /// Searches for a newer version of an entity in the database if it exists and returns it if found.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, default(T).</returns>
        public Task<T> GetNewerVersionIfExists(Guid guid, int version)
        {
            return this.Decorated.GetNewerVersionIfExists(guid, version);
        }

        /// <summary>
        /// Persists a new document representing the entity to the repository if the state is valid.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Insert(T entity)
        {
            this.Validator.ValidateState(default, entity);

            await this.Decorated.Insert(entity);
        }

        /// <summary>
        /// Overwrites an existing document representing <see ref="T" /> to the repository if the state change is valid.
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

            this.Validator.ValidateState(oldEntity, updatedEntity);

            await this.Decorated.Update(updatedEntity);
        }

        /// <summary>
        /// Permanently removes an existing document representing <see ref="T" /> from the repository if the state change is valid.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public async Task Delete(Guid guid)
        {
            var entity = await this.Decorated.GetById(guid);

            this.Validator.ValidateState(entity, default);

            await this.Decorated.Delete(guid);
        }
    }
}
