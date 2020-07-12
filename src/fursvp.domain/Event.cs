// <copyright file="Event.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Fursvp.Domain.Forms;


    /// <summary>
    /// The Domain Event representing the settings and current state of an Event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Can't think of a suitable alternative. If this changes, rename, or correctly map to, any database collection.")]
    public class Event : IEntity<Event>
    {
        /// <summary>
        /// Gets or sets the globally unique identifier for the Event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Version of this entity used for database version control.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the local date and time at which the Event is scheduled to start.
        /// </summary>
        public DateTime StartsAt { get; set; }

        /// <summary>
        /// Gets or sets the local date and time at which the Event is scheduled to end.
        /// </summary>
        public DateTime EndsAt { get; set; }

        /// <summary>
        /// Gets or sets the Event's Time Zone Id.
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the collection of Members associated with this Event.
        /// </summary>
        public ICollection<Member> Members { get; } = new Collection<Member>();

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
        /// Gets or sets the local date and time at which RSVPs for the Event are scheduled to close.
        /// </summary>
        public DateTime? RsvpClosesAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an Event is publicly visible.
        /// </summary>
        public bool IsPublished { get; set; }
    }
}
