// <copyright file="AuthorizeFrozenMemberAsAttendee.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    using System.Linq;
    using Fursvp.Helpers;

    public class AuthorizeFrozenMemberAsAttendee : IAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeFrozenMemberAsAttendee"/> class.
        /// </summary>
        public AuthorizeFrozenMemberAsAttendee()
        {
            this.Assert = new Assertions<NotAuthorizedException<Event>>();
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        public void Authorize(string actor, Member oldState, Member newState)
        {
            this.Assert.That(oldState != null, "New members cannot be added at this time.");
            this.Assert.That(newState != null, "Members cannot be removed at this time.");
            this.Assert.That(oldState.EmailAddress == newState.EmailAddress, "Member info cannot be updated at this time.");
            this.Assert.That(oldState.Name == newState.Name, "Member info cannot be updated at this time.");
            this.Assert.That(oldState.IsAttending == newState.IsAttending, "Member info cannot be updated at this time.");
            this.Assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Member info cannot be updated at this time.");
            this.Assert.That(oldState.IsAuthor == newState.IsAuthor, "Member info cannot be updated at this time.");

            // Assert that the old form responses and new form responses are equivalent.
            foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
            {
                this.Assert.That(formPrompt.old != null && formPrompt.@new != null, "Form responses cannot be updated at this time.");

                var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                {
                    this.Assert.That(option.old != null && option.@new != null, "Form responses cannot be updated at this time.");
                }
            }
        }
    }
}
