using Fursvp.Domain.Authorization.ReadAuthorization;
using Fursvp.Domain.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fursvp.Api.Responses
{
    /// <summary>
    /// A client-consumable representation of the Member's information.
    /// </summary>
    public class MemberResponse : IReadAuthorizableMember
    {
        /// <summary>
        /// Gets or sets the globally unique identifier for an event member.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the email address for an event member.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the display name for an event member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an event member is attending the event.
        /// </summary>
        public bool IsAttending { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an event member is an organizer for the event.
        /// </summary>
        public bool IsOrganizer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an event member is the author of the event.
        /// </summary>
        public bool IsAuthor { get; set; }

        /// <summary>
        /// Gets or sets the collection of form responses for an event attendee.
        /// </summary>
        public ICollection<FormResponses> Responses { get; } = new Collection<FormResponses>();

        /// <summary>
        /// Gets or sets the Utc time that the member was RSVPed to the event.
        /// </summary>
        public DateTime RsvpedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the event-local time that the member was RSVPed to the event.
        /// </summary>
        public DateTime RsvpedAtLocal { get; set; }
    }
}
