// <copyright file="WriteAuthorizeEvent.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    using System.Linq;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Helpers;

    /// <summary>
    /// Checks for authorization by a given actor to create or perform a change to an Event.
    /// </summary>
    public class WriteAuthorizeEvent : IWriteAuthorize<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteAuthorizeEvent"/> class.
        /// </summary>
        /// <param name="writeAuthorizeMember">The <see cref="IWriteAuthorizeMember"/> used to authorize Member state changes.</param>
        /// <param name="userAccessor">An instance of <see cref="IUserAccessor"/> used to get the authenticated user's information..</param>
        public WriteAuthorizeEvent(IWriteAuthorizeMember writeAuthorizeMember, IUserAccessor userAccessor)
        {
            this.Assert = new Assertions<NotAuthorizedException<Event>>();
            this.UserAccessor = userAccessor;
            this.WriteAuthorizeMember = writeAuthorizeMember;
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        private IUserAccessor UserAccessor { get; }

        private IWriteAuthorizeMember WriteAuthorizeMember { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial state of the Event.</param>
        /// <param name="newState">The new state of the Event.</param>
        public void WriteAuthorize(Event oldState, Event newState)
        {
            var actingMember = (oldState ?? newState)?.Members?.FirstOrDefault(m => m.EmailAddress == this.UserAccessor.User?.EmailAddress);

            if (newState == null)
            {
                // Deletion
                this.Assert.That(actingMember?.IsAuthor == true, "An event can be deleted only by its author.");
            }

            if (oldState != null && newState != null)
            {
                foreach (var memberState in oldState.Members.FullJoin(newState.Members, m => m.Id, m => m.Id, (old, @new) => new { old, @new }))
                {
                    this.WriteAuthorizeMember.WriteAuthorize(memberState.old, oldState, memberState.@new, newState, actingMember);
                }

                if (actingMember?.IsAuthor == true || actingMember?.IsOrganizer == true)
                {
                    return;
                }

                this.Assert.That(oldState.StartsAt == newState.StartsAt, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.EndsAt == newState.EndsAt, nameof(oldState.EndsAt) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.TimeZoneId == newState.TimeZoneId, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.Name == newState.Name, nameof(oldState.Name) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.OtherDetails == newState.OtherDetails, nameof(oldState.OtherDetails) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.Location == newState.Location, nameof(oldState.Location) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.RsvpOpen == newState.RsvpOpen, nameof(oldState.RsvpOpen) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.RsvpClosesAt == newState.RsvpClosesAt, nameof(oldState.RsvpClosesAt) + " can only be altered by an event's Author or Organizer.");
                this.Assert.That(oldState.IsPublished == newState.IsPublished, nameof(oldState.IsPublished) + " can only be altered by an event's Author or Organizer.");

                // Assert that the old form and new form are equivalent.
                foreach (var formPrompt in oldState.Form.FullJoin(newState.Form, f => f.Id, f => f.Id, (old, @new) => new { old, @new }))
                {
                    this.Assert.That(formPrompt.old != null && formPrompt.@new != null, "Form can only be altered by an event's Author or Organizer.");
                    this.Assert.That(formPrompt.old.Prompt != formPrompt.@new.Prompt, "Form can only be altered by an event's Author or Organizer.");
                    this.Assert.That(formPrompt.old.Required != formPrompt.@new.Required, "Form can only be altered by an event's Author or Organizer.");
                    this.Assert.That(formPrompt.old.Behavior != formPrompt.@new.Behavior, "Form can only be altered by an event's Author or Organizer.");
                    this.Assert.That(formPrompt.old.SortOrder != formPrompt.@new.SortOrder, "Form can only be altered by an event's Author or Organizer.");

                    var oldOptions = formPrompt.old?.Options ?? Enumerable.Empty<string>();
                    var newOptions = formPrompt.@new?.Options ?? Enumerable.Empty<string>();
                    foreach (var option in oldOptions.FullJoin(newOptions, s => s, s => s, (old, @new) => new { old, @new }))
                    {
                        this.Assert.That(option.old != null && option.@new != null, "Form can only be altered by an event's Author or Organizer.");
                    }
                }
            }
        }
    }
}
