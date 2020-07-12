using System;
using System.Collections.Generic;
using System.Text;

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    public interface IReadAuthorizableMember
    {
        string EmailAddress { get; set; }

        bool IsAuthor { get; }

        bool IsOrganizer { get; }
    }
}
