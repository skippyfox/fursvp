// <copyright file="EventService.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;
    using System.Collections.Generic;

    public class EventService : IEventService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">An instance of <see cref="IProvideDateTime"/>.</param>
        public EventService(IProvideDateTime dateTimeProvider)
        {
            this.DateTimeProvider = dateTimeProvider;
        }

        private IProvideDateTime DateTimeProvider { get; }

        public Event CreateNewEvent(string emailAddress, string name)
        {
            var author = new Member
            {
                Id = Guid.NewGuid(),
                EmailAddress = emailAddress,
                IsAuthor = true,
                IsOrganizer = true,
                IsAttending = true,
                Name = name,
            };

            var @event = new Event()
            {
                Id = Guid.NewGuid(),
                Members = new List<Member> { author },
            };

            return @event;
        }

        public void AddMember(Event @event, Member member)
        {
            member.Id = Guid.NewGuid();
            @event.Members.Add(member);
        }

        public bool RsvpOpen(Event @event)
        {
            return @event.RsvpOpen && this.DateTimeProvider.Now < @event.RsvpClosesAt;
        }

        // TODO: Update with authentication check

        // TODO: Update/Remove Members
    }
}
