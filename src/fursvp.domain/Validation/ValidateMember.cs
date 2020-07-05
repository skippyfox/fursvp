// <copyright file="ValidateMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Fursvp.Helpers;

    /// <summary>
    /// Compares and validates the transition between two states (instances of <see cref="Member"/>). For use with Domain Member validation, not endpoint request validation.
    /// </summary>
    public class ValidateMember : IValidateMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateMember"/> class.
        /// </summary>
        /// <param name="validateEmail">An instance of <see cref="IValidateEmail"/> to provide email validation.</param>
        public ValidateMember(IValidateEmail validateEmail)
        {
            ValidateEmail = validateEmail;
            Assert = new Assertions<ValidationException<Member>>();
        }

        private IValidateEmail ValidateEmail { get; }

        private Assertions<ValidationException<Member>> Assert { get; }

        /// <summary>
        /// Compares two instances of <see cref="Member"/> and throws an exception if the transition from oldState to newState is not valid.
        /// </summary>
        /// <param name="oldMemberState">The old member state.</param>
        /// <param name="oldEventState">The old event state.</param>
        /// <param name="newMemberState">The new member state.</param>
        /// <param name="newEventState">The new event state.</param>
        public void ValidateState(Member oldMemberState, Event oldEventState, Member newMemberState, Event newEventState)
        {
            if (oldMemberState == null && newMemberState == null)
            {
                throw new ArgumentNullException(nameof(newMemberState));
            }

            if (newMemberState == null)
            {
                Contract.Requires(oldMemberState != null);

                Assert.That(!oldMemberState.IsAuthor, "The author of an event cannot be removed.");

                return; // Delete member
            }

            if (newEventState == null)
            {
                // This cannot be null if newMemberState is not null
                throw new ArgumentNullException(nameof(newEventState));
            }

            Assert.That(!string.IsNullOrWhiteSpace(newMemberState.EmailAddress), "Email address is required.");
            ValidateEmail.Validate(newMemberState.EmailAddress);
            Assert.That(!string.IsNullOrWhiteSpace(newMemberState.Name), "Name is required.");
            Assert.That(newMemberState.IsAuthor || newMemberState.IsOrganizer || newMemberState.IsAttending, "Member must be the Author, an Organizer, or Attending.");
            Assert.That(newMemberState.Responses != null, "Responses cannot be null.");
            Assert.That(oldMemberState == null || newMemberState == null || oldMemberState.RsvpedAt == default(DateTime) || oldMemberState.RsvpedAt == newMemberState.RsvpedAt, "RsvpedAt cannot be changed once set.");

            foreach (var response in newMemberState.Responses)
            {
                var formPrompt = newEventState.Form.FirstOrDefault(f => f.Id == response.PromptId);
                Assert.That(formPrompt != null, "Response prompt must have a matching prompt in the event form.");
                Assert.That(!response.Responses.Any(string.IsNullOrWhiteSpace), "Response text cannot be null or whitespace.");

                if (formPrompt.Required)
                {
                    Assert.That(response.Responses.Count > 0, "Form response is required.");
                }

                // TODO - Get these magic strings out of here
                if (formPrompt.Behavior == "Text" || formPrompt.Behavior == "Dropdown")
                {
                    Assert.That(response.Responses.Count <= 1, "Form prompt does not permit multiple answers.");
                }
            }
        }
    }
}
