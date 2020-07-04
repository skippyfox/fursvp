// <copyright file="WriteAuthorizeMemberAsOrganizer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    using System;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Helpers;

    /// <summary>
    /// Checks for authorization by a given event organizer to create or perform a change to a Member.
    /// </summary>
    public class WriteAuthorizeMemberAsOrganizer : IWriteAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteAuthorizeMemberAsOrganizer"/> class.
        /// </summary>
        /// <param name="userAccessor">An instance of <see cref="IUserAccessor"/> used to get the authenticated user's information..</param>
        public WriteAuthorizeMemberAsOrganizer(IUserAccessor userAccessor)
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
            if (oldState != null && (oldState.IsOrganizer || oldState.IsAuthor))
            {
                Assert.That(oldState.EmailAddress == UserAccessor.User?.EmailAddress, "Only the Event Author can modify or remove another organizer's info.");
            }

            if (newState != null)
            {
                Assert.That(!newState.IsAuthor, "This property cannot be set by an organizer.");
                Assert.That(!newState.IsOrganizer, "This property cannot be set by an organizer.");
            }
        }
    }
}
