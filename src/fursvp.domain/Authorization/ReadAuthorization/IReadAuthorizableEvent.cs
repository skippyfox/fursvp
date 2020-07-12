using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    public interface IReadAuthorizableEvent<T> 
        where T : IReadAuthorizableMember
    {
        bool IsPublished { get; }

        ICollection<T> Members { get; }
    }
}
