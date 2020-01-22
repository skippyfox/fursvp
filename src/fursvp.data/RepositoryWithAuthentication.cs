using fursvp.domain;
using fursvp.domain.Authorization;
using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fursvp.data
{
    public class RepositoryWithAuthorization<T> : IRepository<T> where T : IEntity<T>
    {
        private IRepository<T> _decorated { get; }
        private IAuthorize<T> _authorize { get; }

        public RepositoryWithAuthorization(IRepository<T> decorated, IAuthorize<T> authorize)
        {
            _decorated = decorated;
            _authorize = authorize;
        }

        public async Task<IQueryable<T>> GetAll() => await _decorated.GetAll();
        public async Task<T> GetById(Guid guid) => await _decorated.GetById(guid);

        public async Task Insert(T entity)
        {
            _authorize.Authorize("", default, entity);

            await _decorated.Insert(entity);
        }

        public async Task Update(T updatedEntity)
        {
            var oldEntity = await _decorated.GetById(updatedEntity.Id);

            if (oldEntity == null)
                throw new ValidationException<T>("Must provide a valid id");

            _authorize.Authorize("", oldEntity, updatedEntity);

            await _decorated.Update(updatedEntity);
        }

        public async Task Delete(Guid guid)
        {
            var entity = await _decorated.GetById(guid);

            _authorize.Authorize("", entity, default);

            await _decorated.Delete(guid);
        }
    }
}
