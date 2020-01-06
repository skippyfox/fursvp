using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Authorization
{
    public interface IAuthorize<T>
    {
        void Authorize(string actor, T oldState, T newState);
    }
}
