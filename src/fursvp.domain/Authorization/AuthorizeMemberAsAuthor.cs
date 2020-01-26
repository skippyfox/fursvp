// <copyright file="AuthorizeMemberAsAuthor.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    public class AuthorizeMemberAsAuthor : IAuthorize<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeMemberAsAuthor"/> class.
        /// </summary>
        public AuthorizeMemberAsAuthor()
        {
        }

        public void Authorize(string actor, Member oldState, Member newState)
        {
        }
    }
}
