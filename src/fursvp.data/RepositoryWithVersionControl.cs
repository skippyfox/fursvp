using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.data
{
    public class RepositoryWithVersionControl<T> : IRepository<T> where T : IEntity<T>
    {
        private IRepository<T> _decorated { get; }

        public RepositoryWithVersionControl(IRepository<T> decorated)
        {
            _decorated = decorated;
        }

        public void Delete(Guid guid) => _decorated.Delete(guid);
        public IQueryable<T> GetAll() => _decorated.GetAll();
        public T GetById(Guid guid) => _decorated.GetById(guid);
        public void Insert(T entity) => _decorated.Insert(entity);

        public void Update(T entity)
        {
            var oldEntity = _decorated.GetById(entity.Id);
            
            if (oldEntity.Version != entity.Version)
                throw new VersionControlException<T>("Entity versions do not match.");

            entity.Version++;

            _decorated.Update(entity);
        }
    }
}
