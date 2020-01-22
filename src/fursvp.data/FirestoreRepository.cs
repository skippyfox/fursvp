using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fursvp.domain;
using Google.Cloud.Firestore;

namespace fursvp.data
{
    public class FirestoreRepository<T> : IRepository<T>
        where T : IEntity<T>
    {
        private FirestoreDb Db { get; }
        private const string ProjectId = "fursvp-dev";
        private CollectionReference _collection { get; }

        public FirestoreRepository()
        {
            Db = FirestoreDb.Create(ProjectId);
            _collection = Db.Collection(typeof(T).Name);
        }

        private DocumentReference Document(Guid id) => _collection.Document(id.ToString());
        private DocumentReference Document(T entity) => Document(entity.Id);

        public async Task<IQueryable<T>> GetAll()
        {
            //TODO: Can we instantiate some IQueryable<T> that can query the DB with filter on enumerator get?
            // https://cloud.google.com/firestore/docs/query-data/queries
            QuerySnapshot snapshot = await _collection.GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<T>()).AsQueryable();
        }

        public async Task<T> GetById(Guid guid)
        {
            var snapshot = await Document(guid).GetSnapshotAsync();
            if (snapshot.Exists)
                return snapshot.ConvertTo<T>();

            return default;
        }

        public Task Insert(T entity) => Document(entity).CreateAsync(entity);

        public Task Update(T entity) => Document(entity).SetAsync(entity, SetOptions.Overwrite);

        public Task Delete(Guid guid) => Document(guid).DeleteAsync();
    }
}
