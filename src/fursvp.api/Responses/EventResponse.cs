using Fursvp.Domain.Authorization.ReadAuthorization;
using Fursvp.Domain.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fursvp.Api.Responses
{
    /// <summary>
    /// A client-consumable representation of the Event's settings and current state.
    /// </summary>
    public class EventResponse : IReadAuthorizableEvent<MemberResponse>
    {
        /// <summary>
        /// Gets or sets the globally unique identifier for the Event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Version of this entity used for version control.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which the Event is scheduled to start.
        /// </summary>
        public DateTime StartsAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which the Event is scheduled to end.
        /// </summary>
        public DateTime EndsAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the event-local date and time at which the Event is scheduled to start.
        /// </summary>
        public DateTime StartsAtLocal { get; set; }

        /// <summary>
        /// Gets or sets the event-local date and time at which the Event is scheduled to end.
        /// </summary>
        public DateTime EndsAtLocal { get; set; }

        /// <summary>
        /// Gets or sets the Event's Time Zone Id.
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the Event's Time Zone offset from UTC.
        /// </summary>
        public string TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets the collection of Members associated with this Event.
        /// </summary>
        public ICollection<MemberResponse> Members { get; } = new Collection<MemberResponse>();

        /// <summary>
        /// Gets or sets form prompts and options for an Event.
        /// </summary>
        public ICollection<FormPrompt> Form { get; } = new Collection<FormPrompt>();

        /// <summary>
        /// Gets or sets the Event Name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets miscellaneous information about an Event.
        /// </summary>
        public string OtherDetails { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the Event.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether RSVPs for an Event are open before RsvpClosesAt.
        /// </summary>
        public bool RsvpOpen { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which RSVPs for the Event are scheduled to close.
        /// </summary>
        public DateTime? RsvpClosesAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the event-local date and time at which RSVPs for the Event are scheduled to close.
        /// </summary>
        public DateTime? RsvpClosesAtLocal { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds between UtcNow and RsvpClosesAtUtc. Positive if 
        /// </summary>
        public double? RsvpClosesInMs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an Event is publicly visible.
        /// </summary>
        public bool IsPublished { get; set; }
    }
}
