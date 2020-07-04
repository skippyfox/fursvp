// <copyright file="AuthorizeFrozenMemberAsAttendee.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    using System.Linq;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Helpers;

    /// <summary>
    /// Checks for authorization by a given event attendee to create or perform a change to a Member that is frozen based on its Event's settings.
    /// </summary>
    public class WriteAuthorizeFrozenMemberAsAttendee : IWriteAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteAuthorizeFrozenMemberAsAttendee"/> class.
        /// </summary>
        public WriteAuthorizeFrozenMemberAsAttendee()
        {
            Assert = new Assertions<NotAuthorizedException<Event>>();
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial state of the Member.</param>
        /// <param name="newState">The new state of the Member.</param>
        public void WriteAuthorize(Member oldState, Member newState)
        {
            Assert.That(oldState != null, "New members cannot be added at this time.");
            Assert.That(newState != null, "Members cannot be removed at this time.");
            Assert.That(oldState.EmailAddress == newState.EmailAddress, "Member info cannot be updated at this time.");
            Assert.That(oldState.Name == newState.Name, "Member info cannot be updated at this time.");
            Assert.That(oldState.IsAttending == newState.IsAttending, "Member info cannot be updated at this time.");
            Assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Member info cannot be updated at this time.");
            Assert.That(oldState.IsAuthor == newState.IsAuthor, "Member info cannot be updated at this time.");

            // Assert that the old form responses and new form responses are equivalent.
            foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
            {
                Assert.That(formPrompt.old != null && formPrompt.@new != null, "Form responses cannot be updated at this time.");

                var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                {
                    Assert.That(option.old != null && option.@new != null, "Form responses cannot be updated at this time.");
                }
            }
        }
    }
}
