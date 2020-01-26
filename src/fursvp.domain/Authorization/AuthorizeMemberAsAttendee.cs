// <copyright file="AuthorizeMemberAsAttendee.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    using System.Linq;
    using Fursvp.Helpers;

    public class AuthorizeMemberAsAttendee : IAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeMemberAsAttendee"/> class.
        /// </summary>
        public AuthorizeMemberAsAttendee()
        {
            this.Assert = new Assertions<NotAuthorizedException<Event>>();
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        public void Authorize(string actor, Member oldState, Member newState)
        {
            if (oldState != null)
            {
                if (oldState.EmailAddress != actor)
                {
                    this.Assert.That(newState != null, "Only an organizer can remove another member's info.");
                    this.Assert.That(oldState.EmailAddress == newState.EmailAddress, "Only an organizer can modify another member's info.");
                    this.Assert.That(oldState.Name == newState.Name, "Only an organizer can modify another member's info.");
                    this.Assert.That(oldState.IsAttending == newState.IsAttending, "Only an organizer can modify another member's info.");
                    this.Assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Only an organizer can modify another member's info.");
                    this.Assert.That(oldState.IsAuthor == newState.IsAuthor, "Only an organizer can modify another member's info.");

                    // Assert that the old form responses and new form responses are equivalent.
                    foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
                    {
                        this.Assert.That(formPrompt.old != null && formPrompt.@new != null, "Only an organizer can modify another member's form responses.");

                        var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                        var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                        foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                        {
                            this.Assert.That(option.old != null && option.@new != null, "Only an organizer can modify another member's form responses.");
                        }
                    }
                }
            }

            if (newState != null)
            {
                this.Assert.That(!newState.IsAuthor, nameof(newState.IsAuthor) + " cannot be altered by an attendee.");
                this.Assert.That(!newState.IsOrganizer, nameof(newState.IsOrganizer) + " cannot be altered by an attendee.");
            }
        }
    }
}
