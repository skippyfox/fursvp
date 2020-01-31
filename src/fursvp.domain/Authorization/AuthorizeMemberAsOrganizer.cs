// <copyright file="AuthorizeMemberAsOrganizer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    using System;
    using Fursvp.Helpers;

    /// <summary>
    /// Checks for authorization by a given event organizer to create or perform a change to a Member.
    /// </summary>
    public class AuthorizeMemberAsOrganizer : IAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeMemberAsOrganizer"/> class.
        /// </summary>
        /// <param name="userAccessor">An instance of <see cref="IUserAccessor"/> used to get the authenticated user's information..</param>
        public AuthorizeMemberAsOrganizer(IUserAccessor userAccessor)
        {
            this.Assert = new Assertions<NotAuthorizedException<Event>>();
            this.UserAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        private IUserAccessor UserAccessor { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial state of the Member.</param>
        /// <param name="newState">The new state of the Member.</param>
        public void Authorize(Member oldState, Member newState)
        {
            if (oldState != null && (oldState.IsOrganizer || oldState.IsAuthor))
            {
                this.Assert.That(oldState.EmailAddress == this.UserAccessor.User?.EmailAddress, "Only the Event Author can modify or remove another organizer's info.");
            }

            if (newState != null)
            {
                this.Assert.That(!newState.IsAuthor, "This property cannot be set by an organizer.");
                this.Assert.That(!newState.IsOrganizer, "This property cannot be set by an organizer.");
            }
        }
    }
}
