// <copyright file="AuthorizeMemberAsAttendee.cs" company="skippyfox">
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
    public class WriteAuthorizeMemberAsAttendee : IWriteAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteAuthorizeMemberAsAttendee"/> class.
        /// </summary>
        /// <param name="userAccessor">An instance of <see cref="IUserAccessor"/> used to get the authenticated user's information..</param>
        public WriteAuthorizeMemberAsAttendee(IUserAccessor userAccessor)
        {
            Assert = new Assertions<NotAuthorizedException<Event>>();
            UserAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        private IUserAccessor UserAccessor { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial state of the Member.</param>
        /// <param name="newState">The new state of the Member.</param>
        public void WriteAuthorize(Member oldState, Member newState)
        {
            if (oldState != null)
            {
                if (oldState.EmailAddress != UserAccessor.User?.EmailAddress)
                {
                    Assert.That(newState != null, "Only an organizer can remove another member's info.");
                    Assert.That(oldState.EmailAddress == newState.EmailAddress, "Only an organizer can modify another member's info.");
                    Assert.That(oldState.Name == newState.Name, "Only an organizer can modify another member's info.");
                    Assert.That(oldState.IsAttending == newState.IsAttending, "Only an organizer can modify another member's info.");
                    Assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Only an organizer can modify another member's info.");
                    Assert.That(oldState.IsAuthor == newState.IsAuthor, "Only an organizer can modify another member's info.");

                    // Assert that the old form responses and new form responses are equivalent.
                    foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
                    {
                        Assert.That(formPrompt.old != null && formPrompt.@new != null, "Only an organizer can modify another member's form responses.");

                        var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                        var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                        foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                        {
                            Assert.That(option.old != null && option.@new != null, "Only an organizer can modify another member's form responses.");
                        }
                    }
                }
            }

            if (newState != null)
            {
                Assert.That(!newState.IsAuthor, nameof(newState.IsAuthor) + " cannot be altered by an attendee.");
                Assert.That(!newState.IsOrganizer, nameof(newState.IsOrganizer) + " cannot be altered by an attendee.");
            }
        }
    }
}
