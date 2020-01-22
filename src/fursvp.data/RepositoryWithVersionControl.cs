using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fursvp.data
{
    public class RepositoryWithVersionControl<T> : IRepository<T> where T : IEntity<T>
    {
        private IRepository<T> _decorated { get; }

        public RepositoryWithVersionControl(IRepository<T> decorated)
        {
            _decorated = decorated;
        }

        public async Task Delete(Guid guid) => await _decorated.Delete(guid);
        public async Task<IQueryable<T>> GetAll() => await _decorated.GetAll();
        public async Task<T> GetById(Guid guid) => await _decorated.GetById(guid);
        public async Task Insert(T entity) => await _decorated.Insert(entity);

        public async Task Update(T entity)
        {
            var oldEntity = await _decorated.GetById(entity.Id);
            
            if (oldEntity.Version != entity.Version)
                throw new VersionControlException<T>("Entity versions do not match.");

            entity.Version++;

            await _decorated.Update(entity);
        }
    }
}
