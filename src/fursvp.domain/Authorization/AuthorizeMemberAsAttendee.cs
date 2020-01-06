using fursvp.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class AuthorizeMemberAsAttendee : IAuthorize<Member>
    {
        private Assertions<NotAuthorizedException<Event>> _assert { get; }
        public AuthorizeMemberAsAttendee()
        {
            _assert = new Assertions<NotAuthorizedException<Event>>();
        }

        public void Authorize(string actor, Member oldState, Member newState)
        {
            if (oldState != null)
            {
                if (oldState.EmailAddress != actor)
                {
                    _assert.That(newState != null, "Only an organizer can remove another member's info.");
                    _assert.That(oldState.EmailAddress == newState.EmailAddress, "Only an organizer can modify another member's info.");
                    _assert.That(oldState.Name == newState.Name, "Only an organizer can modify another member's info.");
                    _assert.That(oldState.IsAttending == newState.IsAttending, "Only an organizer can modify another member's info.");
                    _assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Only an organizer can modify another member's info.");
                    _assert.That(oldState.IsAuthor == newState.IsAuthor, "Only an organizer can modify another member's info.");

                    //Assert that the old form responses and new form responses are equivalent.
                    foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
                    {
                        _assert.That(formPrompt.old != null && formPrompt.@new != null, "Only an organizer can modify another member's form responses.");

                        var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                        var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                        foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                        {
                            _assert.That(option.old != null && option.@new != null, "Only an organizer can modify another member's form responses.");
                        }
                    }
                }
            }

            if (newState != null)
            {
                _assert.That(!newState.IsAuthor, nameof(newState.IsAuthor) + " cannot be altered by an attendee.");
                _assert.That(!newState.IsOrganizer, nameof(newState.IsOrganizer) + " cannot be altered by an attendee.");
            }
        }
    }
}
