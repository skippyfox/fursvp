// <copyright file="ValidateEvent.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;
    using System.Linq;
    using Fursvp.Helpers;

    /// <summary>
    /// Compares and validates the transition between two states (instances of <see cref="Event"/>). For use with Domain Event validation, not endpoint request validation.
    /// </summary>
    public class ValidateEvent : IValidate<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateEvent"/> class.
        /// </summary>
        /// <param name="validateMember">An instance of <see cref="IValidate{Member}"/> to provide Member state validation.</param>
        /// <param name="validateTimeZone">An instance of <see cref="IValidateTimeZone"/> to provide time zone validation.</param>
        public ValidateEvent(IValidateMember validateMember, IValidateTimeZone validateTimeZone)
        {
            this.ValidateMember = validateMember;
            this.ValidateTimeZone = validateTimeZone;
            this.Assert = new Assertions<ValidationException<Event>>();
        }

        private IValidateTimeZone ValidateTimeZone { get; }

        private IValidateMember ValidateMember { get; }

        private Assertions<ValidationException<Event>> Assert { get; }

        /// <summary>
        /// Compares two instances of <see cref="Event"/> and throws an exception if the transition from oldState to newState is not valid.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public void ValidateState(Event oldState, Event newState)
        {
            if (oldState == null && newState == null)
            {
                throw new ArgumentNullException(nameof(newState));
            }

            this.Assert.That(newState != null, "Deleting an Event is not allowed.");
            this.Assert.That(newState.Members?.Any() == true, "Members list cannot be empty or null.");
            this.Assert.That(newState.Members.Select(x => x.EmailAddress.ToLower()).Distinct().Count() == newState.Members.Count(), "Each member in list must have a unique email address.");
            this.Assert.That(newState.Members.Count(x => x.IsAuthor) == 1, "Members list must contain exactly one author of the event.");

            var oldMembers = oldState?.Members ?? Enumerable.Empty<Member>();
            var newMembers = newState?.Members ?? Enumerable.Empty<Member>();
            foreach (var memberState in oldMembers.FullJoin(newMembers, m => m.Id, m => m.Id, (old, @new) => new { old, @new }))
            {
                this.ValidateMember.ValidateState(memberState.old, oldState, memberState.@new, newState);
            }

            this.Assert.That(newState.Form != null, "Form cannot be null.");

            // TODO: Get these magic strings out of here
            var promptBehaviors = new[] { "Text", "Checkboxes", "Dropdown" };

            foreach (var prompt in newState.Form)
            {
                this.Assert.That(!promptBehaviors.Contains(prompt.Behavior), "A prompt behavior must be one of: " + string.Join(", ", promptBehaviors));
                switch (prompt.Behavior)
                {
                    case "Text":
                        this.Assert.That(!prompt.Options.Any(), "A text prompt cannot have any options.");
                        break;
                    case "Checkboxes":
                        this.Assert.That(prompt.Options.Count >= 1, "A checkboxes prompt must have at least one option.");
                        break;
                    case "Dropdown":
                        this.Assert.That(prompt.Options.Count >= 2, "A dropdown prompt must have at least two options.");
                        break;
                }
            }

            if (!newState.RsvpOpen)
            {
                this.Assert.That(!newState.RsvpClosesAt.HasValue, "RsvpClosesAt cannot be set if Rsvp is closed.");
            }

            if (newState.RsvpOpen)
            {
                this.Assert.That(newState.RsvpClosesAt.HasValue, "RsvpClosesAt must be set if Rsvp is open.");
            }

            if (newState.IsPublished)
            {
                this.Assert.That(newState.StartsAt != default, "Event must have a start date and time.");
                this.Assert.That(newState.EndsAt != default, "Event must have an end date and time.");
                this.Assert.That(newState.StartsAt < newState.EndsAt, "Start time must be before End time.");

                this.Assert.That(newState.TimeZoneId != null, "Time Zone is required.");
                this.ValidateTimeZone.Validate(newState.TimeZoneId);
            }
        }
    }
}
