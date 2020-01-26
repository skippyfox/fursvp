using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fursvp.domain;
using Google.Cloud.Firestore;

namespace fursvp.data.Firestore
{
    public class FirestoreRepository<T> : IRepository<T>
        where T : IEntity<T>
    {
        private FirestoreDb Db { get; }
        private const string ProjectId = "fursvp-dev";
        private CollectionReference _collection { get; }
        private IDictionaryMapper<T> _mapper { get; }

        public FirestoreRepository(IDictionaryMapper<T> mapper)
        {
            Db = FirestoreDb.Create(ProjectId);
            _collection = Db.Collection(typeof(T).Name);
            _mapper = mapper;
        }

        private DocumentReference Document(Guid id) => _collection.Document(id.ToString());
        private DocumentReference Document(T entity) => Document(entity.Id);

        public async Task<IQueryable<T>> GetAll()
        {
            //TODO: Can we instantiate some IQueryable<T> that can query the DB with filter on enumerator get?
            // https://cloud.google.com/firestore/docs/query-data/queries
            QuerySnapshot snapshot = await _collection.GetSnapshotAsync();
            return snapshot.Documents.Select(d => _mapper.FromDictionary(d.ToDictionary())).AsQueryable();
        }

        public async Task<T> GetById(Guid guid)
        {
            var snapshot = await Document(guid).GetSnapshotAsync();
            if (snapshot.Exists)
                return _mapper.FromDictionary(snapshot.ToDictionary());

            return default;
        }

        public Task Insert(T entity) => Document(entity).CreateAsync(_mapper.ToDictionary(entity));

        public Task Update(T entity) => Document(entity).SetAsync(_mapper.ToDictionary(entity), SetOptions.Overwrite);

        public Task Delete(Guid guid) => Document(guid).DeleteAsync();
    }
}
