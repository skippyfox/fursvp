using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Validation
{
    public interface IValidateEntity<T> where T : IEntity<T>
    {
        void ValidateState(T oldState, T newState);
        void ValidateDelete(T entity);
    }
}
