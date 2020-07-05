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
        private const string ProjectId = "fursvp-dev"; // TODO - put into config variable

        /// <summary>
        /// Initializes a new instance of the <see cref="FirestoreRepository{T}"/> class.
        /// </summary>
        /// <param name="mapper">An instance of <see cref="IDictionaryMapper{T}"/> to map between the domain entity and the Firestore document.</param>
        public FirestoreRepository(IDictionaryMapper<T> mapper)
        {
            Db = FirestoreDb.Create(ProjectId);
            Collection = Db.Collection(typeof(T).Name);
            Mapper = mapper;
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
            QuerySnapshot snapshot = await Collection.GetSnapshotAsync().ConfigureAwait(false);
            return snapshot.Documents.Select(d => Mapper.FromDictionary(d.ToDictionary())).AsQueryable();
        }

        /// <summary>
        /// Gets a single instance of T matching the given Id from Firestore.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/> containing the entity if found, otherwise null.</returns>
        public async Task<T> GetById(Guid guid)
        {
            var snapshot = await Document(guid).GetSnapshotAsync().ConfigureAwait(false);
            if (snapshot.Exists)
            {
                return Mapper.FromDictionary(snapshot.ToDictionary());
            }

            return default;
        }

        /// <summary>
        /// Searches for a newer version of an entity in the database if it exists and returns it if found.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, default(T).</returns>
        public async Task<T> GetNewerVersionIfExists(Guid guid, int version)
        {
            var snapshotCollection = await Collection
                .WhereEqualTo(new FieldPath("Id"), guid.ToString())
                .WhereGreaterThan(new FieldPath("Version"), version)
                .Limit(1)
                .GetSnapshotAsync().ConfigureAwait(false);

            if (snapshotCollection.Count > 0 && snapshotCollection[0]?.Exists == true)
            {
                return Mapper.FromDictionary(snapshotCollection[0].ToDictionary());
            }

            return default;
        }

        /// <summary>
        /// Persists a new document representing the entity to Firestore.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Insert(T entity) => Document(entity).CreateAsync(Mapper.ToDictionary(entity));

        /// <summary>
        /// Overwrites an existing document representing the entity to Firestore.
        /// </summary>
        /// <param name="entity">The entity to persist to the repository.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Update(T entity) => Document(entity).SetAsync(Mapper.ToDictionary(entity), SetOptions.Overwrite);

        /// <summary>
        /// Permanently removes an existing document representing the entity from Firestore.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{T}"/>.</returns>
        public Task Delete(Guid guid) => Document(guid).DeleteAsync();

        private DocumentReference Document(Guid id) => Collection.Document(id.ToString());

        private DocumentReference Document(T entity) => Document(entity.Id);
    }
}
