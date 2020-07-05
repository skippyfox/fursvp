// <copyright file="WriteAuthorizeMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    using System;
    using System.Linq;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Helpers;

    /// <summary>
    /// Checks for authorization by a given event attendee to create or perform a change to a Member.
    /// </summary>
    public class WriteAuthorizeMember : IWriteAuthorizeMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteAuthorizeMember"/> class.
        /// </summary>
        /// <param name="eventService">An instance of <see cref="IEventService"/>.</param>
        public WriteAuthorizeMember(IEventService eventService)
        {
            this.Assert = new Assertions<NotAuthorizedException<Event>>();
            this.EventService = eventService;
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        private IEventService EventService { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldMemberState">The initial member state.</param>
        /// <param name="oldEventState">The initial event state.</param>
        /// <param name="newMemberState">The new member state.</param>
        /// <param name="newEventState">The new event state.</param>
        /// <param name="actingMember">The member performing the action.</param>
        public void WriteAuthorize(Member oldMemberState, Event oldEventState, Member newMemberState, Event newEventState, Member actingMember)
        {
            // I am the event author. I am authorized to do just about anything to member records.
            if (actingMember?.IsAuthor == true)
            {
                return;
            }

            // I am not the author. I am not allowed to change anyone's status as organizer or author, not even my own.
            if (newMemberState != null)
            {
                this.Assert.That(!newMemberState.IsAuthor, nameof(newMemberState.IsAuthor) + " cannot be set.");
                bool targetMemberWasOrganizer = oldMemberState?.IsOrganizer == true;
                this.Assert.That(targetMemberWasOrganizer == newMemberState.IsOrganizer, nameof(newMemberState.IsOrganizer) + " cannot be set.");
            }

            // I'm not an author or organizer and I'm trying to add a new RSVP
            if (oldMemberState == null && actingMember?.IsOrganizer != true)
            {
                this.Assert.That(this.EventService.RsvpOpen(newEventState), "New members cannot be added at this time.");
            }

            // I am editing my own entry or creating a new entry. I can change anything except the above.
            if (oldMemberState == null || oldMemberState == actingMember)
            {
                return;
            }

            // I am an organizer changing or removing an entry that does not belong to another organizer or the author. I can change anything except the above.
            if (actingMember?.IsOrganizer == true && !oldMemberState.IsAuthor && !oldMemberState.IsOrganizer)
            {
                return;
            }

            // I am either an organizer changing or removing another organizer or the author, or I am a member changing or removing anyone else's entry. Changes aren't allowed.
            this.Assert.That(newMemberState != null, "You do not have permission to remove this member's info.");

            this.Assert.That(
                oldMemberState.EmailAddress == newMemberState.EmailAddress
                    && oldMemberState.Name == newMemberState.Name
                    && oldMemberState.IsAttending == newMemberState.IsAttending,
                "You do not have permission to change this member's info.");

            // Assert that the old form responses and new form responses are equivalent.
            var oldResponses = oldMemberState.Responses.SelectMany(r => r.Responses.Select(response => new { r.PromptId, response }));
            var newResponses = newMemberState.Responses.SelectMany(r => r.Responses.Select(response => new { r.PromptId, response }));
            this.Assert.That(oldResponses.FullJoin(newResponses, s => s, s => s, (o, i) => o == null || i == null).Any(), "You do not have permission to change this member's form responses.");
        }
    }
}
