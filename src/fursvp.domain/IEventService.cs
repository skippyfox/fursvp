// <copyright file="IEventService.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    /// <summary>
    /// Provides business logic for creation and updates of an instance of <see cref="Event"/>.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Adds a <see cref="Member"/> to an <see cref="Event"/>.
        /// </summary>
        /// <param name="eventInfo">The <see cref="Event"/> to add a new <see cref="Member"/> to.</param>
        /// <param name="member">The <see cref="Member"/> to be added to an existing <see cref="Event"/>.</param>
        void AddMember(Event eventInfo, Member member);

        /// <summary>
        /// Creates a new <see cref="Event"/>.
        /// </summary>
        /// <param name="emailAddress">The Event author's email address.</param>
        /// <param name="name">The Event author's name.</param>
        /// <returns>The newly created <see cref="Event"/>.</returns>
        Event CreateNewEvent(string emailAddress, string name);

        /// <summary>
        /// Determines whether attendees can RSVP to an <see cref="Event"/>.
        /// </summary>
        /// <param name="eventInstance">The <see cref="Event"/>.</param>
        /// <returns>True if attendees can currently RSVP, otherwise False.</returns>
        bool RsvpOpen(Event eventInstance);
    }
}