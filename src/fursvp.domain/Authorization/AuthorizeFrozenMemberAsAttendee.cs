using fursvp.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class AuthorizeFrozenMemberAsAttendee : IAuthorize<Member>
    {
        private Assertions<NotAuthorizedException<Event>> _assert { get; }
        public AuthorizeFrozenMemberAsAttendee()
        {
            _assert = new Assertions<NotAuthorizedException<Event>>();
        }

        public void Authorize(string actor, Member oldState, Member newState)
        {
            _assert.That(oldState != null, "New members cannot be added at this time.");
            _assert.That(newState != null, "Members cannot be removed at this time.");
            _assert.That(oldState.EmailAddress == newState.EmailAddress, "Member info cannot be updated at this time.");
            _assert.That(oldState.Name == newState.Name, "Member info cannot be updated at this time.");
            _assert.That(oldState.IsAttending == newState.IsAttending, "Member info cannot be updated at this time.");
            _assert.That(oldState.IsOrganizer == newState.IsOrganizer, "Member info cannot be updated at this time.");
            _assert.That(oldState.IsAuthor == newState.IsAuthor, "Member info cannot be updated at this time.");

            //Assert that the old form responses and new form responses are equivalent.
            foreach (var formPrompt in oldState.Responses.FullJoin(newState.Responses, r => r.Prompt, r => r.Prompt, (old, @new) => new { old, @new }))
            {
                _assert.That(formPrompt.old != null && formPrompt.@new != null, "Form responses cannot be updated at this time.");

                var oldResponses = formPrompt.old?.Responses ?? Enumerable.Empty<string>();
                var newResponses = formPrompt.@new?.Responses ?? Enumerable.Empty<string>();
                foreach (var option in oldResponses.FullJoin(newResponses, s => s, s => s, (old, @new) => new { old, @new }))
                {
                    _assert.That(option.old != null && option.@new != null, "Form responses cannot be updated at this time.");
                }
            }
        }
    }
}
