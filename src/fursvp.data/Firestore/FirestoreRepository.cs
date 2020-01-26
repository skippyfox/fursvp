// <copyright file="FirestoreRepository.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.Firestore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Domain;
    using Google.Cloud.Firestore;

    /// <summary>
    /// Interfaces with Google Firestore to access documents representing a domain entity.
    /// </summary>
    /// <typeparam name="T">The domain entity being stored and retrieved using Google Firestore.</typeparam>
    public class FirestoreRepository<T> : IRepository<T>
        where T : IEntity<T>
    {
        private const string ProjectId = "fursvp-dev";

        /// <summary>
        /// Initializes a new instance of the <see cref="FirestoreRepository{T}"/> class.
        /// </summary>
        /// <param name="mapper">An instance of <see cref="IDictionaryMapper{T}"/> to map between the domain entity and the Firestore document.</param>
        public FirestoreRepository(IDictionaryMapper<T> mapper)
        {
            this.Db = FirestoreDb.Create(ProjectId);
            this.Collection = this.Db.Collection(typeof(T).Name);
            this.Mapper = mapper;
        }

        private FirestoreDb Db { get; }

        private CollectionReference Collection { get; }

        private IDictionaryMapper<T> Mapper { get; }

        /// <summary>
        /// Gets a result set from Firestore containing all documents for the entity type.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> against which further filtering can be applied on the result set.</returns>
        public async Task<IQueryable<T>> GetAll()
        {
            // TODO: Can we instantiate some IQueryable<T> that can query the DB with filter on enumerator get?
            // https://cloud.google.com/firestore/docs/query-data/queries
            QuerySnapshot snapshot = await this.Collection.GetSnapshotAsync();
            return snapshot.Documents.Select(d => this.Mapper.FromDictionary(d.ToDictionary())).AsQueryable();
        }

        /// <summary>
        /// Gets a single instance of T matching the given Id from Firestore.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        public async Task<T> GetById(Guid guid)
        {
            var snapshot = await this.Document(guid).GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return this.Mapper.FromDictionary(snapshot.ToDictionary());
            }

            return default;
        }

        /// <summary>
        /// Persists a new document representing the entity to Firestore.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Insert(T entity) => this.Document(entity).CreateAsync(this.Mapper.ToDictionary(entity));

        /// <summary>
        /// Overwrites an existing document representing the entity to Firestore.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Update(T entity) => this.Document(entity).SetAsync(this.Mapper.ToDictionary(entity), SetOptions.Overwrite);

        /// <summary>
        /// Permanently removes an existing document representing the entity from Firestore.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Delete(Guid guid) => this.Document(guid).DeleteAsync();

        private DocumentReference Document(Guid id) => this.Collection.Document(id.ToString());

        private DocumentReference Document(T entity) => this.Document(entity.Id);
    }
}
