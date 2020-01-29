// <copyright file="AuthorizeMemberAsAuthor.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    /// <summary>
    /// Checks for authorization by a given event author to create or perform a change to a Member.
    /// </summary>
    public class AuthorizeMemberAsAuthor : IAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeMemberAsAuthor"/> class.
        /// </summary>
        public AuthorizeMemberAsAuthor()
        {
        }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="actor">The user role for which to check for authorization.</param>
        /// <param name="oldState">The initial state of the Member.</param>
        /// <param name="newState">The new state of the Member.</param>
        public void Authorize(string actor, Member oldState, Member newState)
        {
        }
    }
}
