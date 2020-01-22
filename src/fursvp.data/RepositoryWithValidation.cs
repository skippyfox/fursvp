using fursvp.domain;
using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fursvp.data
{
    public class RepositoryWithValidation<T> : IRepository<T> where T : IEntity<T>
    {
        private IRepository<T> _decorated { get; }
        private IValidate<T> _validator { get; }

        public RepositoryWithValidation(IRepository<T> decorated, IValidate<T> validator)
        {
            _decorated = decorated;
            _validator = validator;
        }

        public async Task<IQueryable<T>> GetAll() => await _decorated.GetAll();
        public async Task<T> GetById(Guid guid) => await _decorated.GetById(guid);

        public async Task Insert(T entity)
        {
            _validator.ValidateState(default, entity);

            await _decorated.Insert(entity);
        }

        public async Task Update(T updatedEntity)
        {
            var oldEntity = await _decorated.GetById(updatedEntity.Id);

            if (oldEntity == null)
                throw new ValidationException<T>("Must provide a valid id");

            _validator.ValidateState(oldEntity, updatedEntity);

            await _decorated.Update(updatedEntity);
        }

        public async Task Delete(Guid guid)
        {
            var entity = await _decorated.GetById(guid);

            _validator.ValidateState(entity, default);

            await _decorated.Delete(guid);
        }
    }
}
