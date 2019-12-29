using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain
{
    public interface IEntity<T> where T : IEntity<T>
    {
        Guid Id { get; set; }
        int Version { get; set; }
    }
}
