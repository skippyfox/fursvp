// <copyright file="IValidateMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    /// <summary>
    /// Compares and validates the transition between two states (instances of <see cref="Member"/>). For use with Domain Member validation, not endpoint request validation.
    /// </summary>
    public interface IValidateMember
    {
        /// <summary>
        /// Compares two instances of <see cref="Member"/> and throws an exception if the transition from oldState to newState is not valid.
        /// </summary>
        /// <param name="oldMemberState">The old member state.</param>
        /// <param name="oldEventState">The old event state.</param>
        /// <param name="newMemberState">The new member state.</param>
        /// <param name="newEventState">The new event state.</param>
        void ValidateState(Member oldMemberState, Event oldEventState, Member newMemberState, Event newEventState);
    }
}
