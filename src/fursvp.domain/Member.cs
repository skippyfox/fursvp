// <copyright file="Member.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using Fursvp.Domain.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A Member of an Event.
    /// </summary>
    public class Member
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
        public ICollection<FormResponses> Responses { get; set; } = new Collection<FormResponses>();
    }
}
