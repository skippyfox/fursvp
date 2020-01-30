﻿// <copyright file="EventService.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides business logic for creation and updates of an instance of <see cref="Event"/>.
    /// </summary>
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

        /// <summary>
        /// Creates a new <see cref="Event"/>.
        /// </summary>
        /// <param name="emailAddress">The Event author's email address.</param>
        /// <param name="name">The Event author's name.</param>
        /// <returns>The newly created <see cref="Event"/>.</returns>
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

        /// <summary>
        /// Adds a <see cref="Member"/> to an <see cref="Event"/>.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to add a new <see cref="Member"/> to.</param>
        /// <param name="member">The <see cref="Member"/> to be added to an existing <see cref="Event"/>.</param>
        public void AddMember(Event @event, Member member)
        {
            member.Id = Guid.NewGuid();
            @event.Members.Add(member);
        }

        /// <summary>
        /// Determines whether attendees can RSVP to an <see cref="Event"/>.
        /// </summary>
        /// <param name="event">The <see cref="Event"/>.</param>
        /// <returns>True if attendees can currently RSVP, otherwise False.</returns>
        public bool RsvpOpen(Event @event)
        {
            return @event.RsvpOpen && this.DateTimeProvider.Now < @event.RsvpClosesAt;
        }

        // TODO: Update/Remove Members
    }
}
