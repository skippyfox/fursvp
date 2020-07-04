// <copyright file="AuthorizeEvent.cs" company="skippyfox">
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
        /// <param name="authorizeMemberAsAuthor">The <see cref="IWriteAuthorize{Member}"/> used to authorize Member state changes when the authenticated user is the Event's author.</param>
        /// <param name="authorizeMemberAsOrganizer">The <see cref="IWriteAuthorize{Member}"/> used to authorize Member state changes when the authenticated user is an organizer of the Event.</param>
        /// <param name="authorizeMemberAsAttendee">The <see cref="IWriteAuthorize{Member}"/> used to authorize Member state changes when the authenticated user is an attendee and RSVPs are open.</param>
        /// <param name="authorizeFrozenMemberAsAttendee">The <see cref="IWriteAuthorize{Member}"/> used to authorize Member state changes when the authenticated user is an attendee and RVSPs are not open.</param>
        /// <param name="eventService">An instance of <see cref="IEventService"/> used to evaluate an Event's state.</param>
        /// <param name="userAccessor">An instance of <see cref="IUserAccessor"/> used to get the authenticated user's information..</param>
        public WriteAuthorizeEvent(
            IWriteAuthorize<Member> authorizeMemberAsAuthor,
            IWriteAuthorize<Member> authorizeMemberAsOrganizer,
            IWriteAuthorize<Member> authorizeMemberAsAttendee,
            IWriteAuthorize<Member> authorizeFrozenMemberAsAttendee,
            IEventService eventService,
            IUserAccessor userAccessor)
        {
            Assert = new Assertions<NotAuthorizedException<Event>>();
            AuthorizeMemberAsAuthor = authorizeMemberAsAuthor;
            AuthorizeMemberAsOrganizer = authorizeMemberAsOrganizer;
            AuthorizeMemberAsAttendee = authorizeMemberAsAttendee;
            AuthorizeFrozenMemberAsAttendee = authorizeFrozenMemberAsAttendee;
            EventService = eventService;
            UserAccessor = userAccessor;
        }

        private Assertions<NotAuthorizedException<Event>> Assert { get; }

        private IWriteAuthorize<Member> AuthorizeMemberAsAttendee { get; }

        private IWriteAuthorize<Member> AuthorizeMemberAsOrganizer { get; }

        private IWriteAuthorize<Member> AuthorizeMemberAsAuthor { get; }

        private IWriteAuthorize<Member> AuthorizeFrozenMemberAsAttendee { get; }

        private IEventService EventService { get; }

        private IUserAccessor UserAccessor { get; }

        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial state of the Event.</param>
        /// <param name="newState">The new state of the Event.</param>
        public void WriteAuthorize(Event oldState, Event newState)
        {
            var actingMember = (oldState ?? newState)?.Members?.FirstOrDefault(m => m.EmailAddress == UserAccessor.User?.EmailAddress);

            if (newState == null)
            {
                // Deletion
                Assert.That(actingMember?.IsAuthor == true, "An event can be deleted only by its author.");
            }

            if (oldState != null && newState != null)
            {
                // Update
                void AuthorizeMembers(IWriteAuthorize<Member> authorizeMember)
                {
                    foreach (var memberState in oldState.Members.FullJoin(newState.Members, m => m.Id, m => m.Id, (old, @new) => new { old, @new }))
                    {
                        authorizeMember.WriteAuthorize(memberState.old, memberState.@new);
                    }
                }

                if (actingMember?.IsAuthor == true)
                {
                    AuthorizeMembers(this.AuthorizeMemberAsAuthor);
                }
                else if (actingMember?.IsOrganizer == true)
                {
                    AuthorizeMembers(this.AuthorizeMemberAsOrganizer);
                }
                else
                {
                    if (EventService.RsvpOpen(newState))
                    {
                        AuthorizeMembers(this.AuthorizeMemberAsAttendee);
                    }
                    else
                    {
                        AuthorizeMembers(this.AuthorizeFrozenMemberAsAttendee);
                    }

                    Assert.That(oldState.StartsAt == newState.StartsAt, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.EndsAt == newState.EndsAt, nameof(oldState.EndsAt) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.TimeZoneId == newState.TimeZoneId, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.Name == newState.Name, nameof(oldState.Name) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.OtherDetails == newState.OtherDetails, nameof(oldState.OtherDetails) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.Location == newState.Location, nameof(oldState.Location) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.RsvpOpen == newState.RsvpOpen, nameof(oldState.RsvpOpen) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.RsvpClosesAt == newState.RsvpClosesAt, nameof(oldState.RsvpClosesAt) + " can only be altered by an event's Author or Organizer.");
                    Assert.That(oldState.IsPublished == newState.IsPublished, nameof(oldState.IsPublished) + " can only be altered by an event's Author or Organizer.");

                    // Assert that the old form and new form are equivalent.
                    foreach (var formPrompt in oldState.Form.FullJoin(newState.Form, f => f.Prompt, f => f.Prompt, (old, @new) => new { old, @new }))
                    {
                        Assert.That(formPrompt.old != null && formPrompt.@new != null, "Form can only be altered by an event's Author or Organizer.");

                        var oldOptions = formPrompt.old?.Options ?? Enumerable.Empty<string>();
                        var newOptions = formPrompt.@new?.Options ?? Enumerable.Empty<string>();
                        foreach (var option in oldOptions.FullJoin(newOptions, s => s, s => s, (old, @new) => new { old, @new }))
                        {
                            Assert.That(option.old != null && option.@new != null, "Form can only be altered by an event's Author or Organizer.");
                        }
                    }
                }
            }
        }
    }
}
