using fursvp.domain;
using fursvp.domain.Authorization;
using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public IQueryable<T> GetAll() => _decorated.GetAll();
        public T GetById(Guid guid) => _decorated.GetById(guid);

        public void Insert(T entity)
        {
            _authorize.Authorize("", default, entity);

            _decorated.Insert(entity);
        }

        public void Update(T updatedEntity)
        {
            var oldEntity = _decorated.GetById(updatedEntity.Id);

            if (oldEntity == null)
                throw new ValidationException<T>("Must provide a valid id");

            _authorize.Authorize("", oldEntity, updatedEntity);

            _decorated.Update(updatedEntity);
        }

        public void Delete(Guid guid)
        {
            var entity = _decorated.GetById(guid);

            _authorize.Authorize("", entity, default);

            _decorated.Delete(guid);
        }
    }
}
