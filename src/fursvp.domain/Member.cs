using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fursvp.domain
{
    public class Member
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public bool IsAttending { get; set; }
        public bool IsOrganizer { get; set; }
        public bool IsAuthor { get; set; }
        public ICollection<FormResponses> Responses { get; set; } = new Collection<FormResponses>();
    }
}
