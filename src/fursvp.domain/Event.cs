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

    public class Event : IEntity<Event>
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public string TimeZoneId { get; set; }

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
