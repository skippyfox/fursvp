using fursvp.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class AuthorizeEvent : IAuthorize<Event>
    {
        private Assertions<NotAuthorizedException<Event>> _assert { get; }
        private IAuthorize<Member> _authorizeMemberAsAttendee { get; }
        private IAuthorize<Member> _authorizeMemberAsOrganizer { get; }
        private IAuthorize<Member> _authorizeMemberAsAuthor { get; }
        private IAuthorize<Member> _authorizeFrozenMemberAsAttendee { get; }
        private IEventService _eventService { get; }
        public AuthorizeEvent(IAuthorize<Member> authorizeMemberAsAuthor, IAuthorize<Member> authorizeMemberAsOrganizer,
            IAuthorize<Member> authorizeMemberAsAttendee, IAuthorize<Member> authorizeFrozenMemberAsAttendee,
            IEventService eventService)
        {
            _assert = new Assertions<NotAuthorizedException<Event>>();
            _authorizeMemberAsAuthor = authorizeMemberAsAuthor;
            _authorizeMemberAsOrganizer = authorizeMemberAsOrganizer;
            _authorizeMemberAsAttendee = authorizeMemberAsAttendee;
            _authorizeFrozenMemberAsAttendee = authorizeFrozenMemberAsAttendee;
            _eventService = eventService;
        }

        public void Authorize(string actor, Event oldState, Event newState)
        {
            var actingMember = (oldState ?? newState)?.Members?.FirstOrDefault(m => m.EmailAddress == actor);

            if (newState == null)
            { //Deletion
                _assert.That(actingMember?.IsAuthor == true, "An event can be deleted only by its author.");
            }

            if (oldState != null && newState != null)
            { //Update

                void AuthorizeMembers(IAuthorize<Member> authorizeMember)
                {
                    foreach (var memberState in oldState.Members.FullJoin(newState.Members, m => m.Id, m => m.Id, (old, @new) => new { old, @new }))
                    {
                        authorizeMember.Authorize(actor, memberState.old, memberState.@new);
                    }
                }

                if (actingMember?.IsAuthor == true)
                {
                    AuthorizeMembers(_authorizeMemberAsAuthor);
                }
                else if (actingMember?.IsOrganizer == true)
                {
                    AuthorizeMembers(_authorizeMemberAsOrganizer);
                }
                else
                {
                    if (_eventService.RsvpOpen(newState))
                    {
                        AuthorizeMembers(_authorizeMemberAsAttendee);
                    }
                    else
                    {
                        AuthorizeMembers(_authorizeFrozenMemberAsAttendee);
                    }
                    _assert.That(oldState.StartsAt == newState.StartsAt, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.EndsAt == newState.EndsAt, nameof(oldState.EndsAt) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.TimeZoneId == newState.TimeZoneId, nameof(oldState.StartsAt) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.Name == newState.Name, nameof(oldState.Name) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.OtherDetails == newState.OtherDetails, nameof(oldState.OtherDetails) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.Location == newState.Location, nameof(oldState.Location) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.RsvpOpen == newState.RsvpOpen, nameof(oldState.RsvpOpen) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.RsvpClosesAt == newState.RsvpClosesAt, nameof(oldState.RsvpClosesAt) + " can only be altered by an event's Author or Organizer.");
                    _assert.That(oldState.IsPublished == newState.IsPublished, nameof(oldState.IsPublished) + " can only be altered by an event's Author or Organizer.");

                    //Assert that the old form and new form are equivalent.
                    foreach (var formPrompt in oldState.Form.FullJoin(newState.Form, f => f.Prompt, f => f.Prompt, (old, @new) => new { old, @new }))
                    {
                        _assert.That(formPrompt.old != null && formPrompt.@new != null, "Form can only be altered by an event's Author or Organizer.");

                        var oldOptions = formPrompt.old?.Options ?? Enumerable.Empty<string>();
                        var newOptions = formPrompt.@new?.Options ?? Enumerable.Empty<string>();
                        foreach (var option in oldOptions.FullJoin(newOptions, s=>s, s=>s, (old, @new) => new { old, @new }))
                        {
                            _assert.That(option.old != null && option.@new != null, "Form can only be altered by an event's Author or Organizer.");
                        }
                    }
                }
            }
        }
    }
}
