// <copyright file="IWriteAuthorizeMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Checks for authorization by a given actor to perform a change to a Member.
    /// </summary>
    public interface IWriteAuthorizeMember
    {
        /// <summary>
        /// Performs the authorization check for a Member state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldMemberState">The initial member state.</param>
        /// <param name="oldEventState">The initial event state.</param>
        /// <param name="newMemberState">The new member state.</param>
        /// <param name="newEventState">The new event state.</param>
        /// <param name="actingMember">The member performing the action.</param>
        void WriteAuthorize(Member oldMemberState, Event oldEventState, Member newMemberState, Event newEventState, Member actingMember);
    }
}
