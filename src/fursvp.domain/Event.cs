using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fursvp.domain
{
    public class Event : IEntity<Event>
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public ICollection<Member> Members { get; set; } = new Collection<Member>();
        public ICollection<FormPrompt> Form { get; set; } = new Collection<FormPrompt>();
        public string Name { get; set; } = string.Empty;
        public string OtherDetails { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool RsvpOpen { get; set; }
        public DateTime? RsvpClosesAt { get; set; }
        public bool IsPublished { get; set; }
    }
}
