using fursvp.domain;
using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.data
{
    public class RepositoryWithValidation<T> : IRepository<T> where T : IEntity<T>
    {
        private IRepository<T> _decorated { get; }
        private IValidateEntity<T> _validator { get; }

        public RepositoryWithValidation(IRepository<T> decorated, IValidateEntity<T> validator)
        {
            _decorated = decorated;
            _validator = validator;
        }

        public IQueryable<T> GetAll() => _decorated.GetAll();
        public T GetById(Guid guid) => _decorated.GetById(guid);

        public void Insert(T entity)
        {
            _validator.ValidateState(default(T), entity);

            _decorated.Insert(entity);
        }

        public void Update(T updatedEntity)
        {
            var oldEntity = _decorated.GetById(updatedEntity.Id);

            if (oldEntity == null)
                throw new ValidationException<T>("Must provide a valid id");

            _validator.ValidateState(oldEntity, updatedEntity);

            _decorated.Update(updatedEntity);
        }

        public void Delete(Guid guid)
        {
            var entity = _decorated.GetById(guid);

            _validator.ValidateDelete(entity);

            _decorated.Delete(guid);
        }
    }
}
