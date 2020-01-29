// <copyright file="ValidateMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;
    using Fursvp.Helpers;

    /// <summary>
    /// Compares and validates the transition between two states (instances of <see cref="Member"/>). For use with Domain Member validation, not endpoint request validation.
    /// </summary>
    public class ValidateMember : IValidate<Member>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateMember"/> class.
        /// </summary>
        /// <param name="validateEmail">An instance of <see cref="IValidateEmail"/> to provide email validation.</param>
        public ValidateMember(IValidateEmail validateEmail)
        {
            this.ValidateEmail = validateEmail;
            this.Assert = new Assertions<ValidationException<Member>>();
        }

        private IValidateEmail ValidateEmail { get; }

        private Assertions<ValidationException<Member>> Assert { get; }

        /// <summary>
        /// Compares two instances of <see cref="Member"/> and throws an exception if the transition from oldState to newState is not valid.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public void ValidateState(Member oldState, Member newState)
        {
            if (oldState == null && newState == null)
            {
                throw new ArgumentNullException(nameof(newState));
            }

            if (newState == null)
            {
                return; // Delete member
            }

            this.Assert.That(!string.IsNullOrWhiteSpace(newState.EmailAddress), "Email address is required.");
            this.ValidateEmail.Validate(newState.EmailAddress);
            this.Assert.That(!string.IsNullOrWhiteSpace(newState.Name), "Name is required.");
            this.Assert.That(newState.IsAuthor || newState.IsOrganizer || newState.IsAttending, "Member must be the Author, an Organizer, or Attending.");
            this.Assert.That(newState.Responses != null, "Responses cannot be null.");
        }
    }
}
