using fursvp.helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class AuthorizeMemberAsOrganizer : IAuthorize<Member>
    {
        private Assertions<NotAuthorizedException<Event>> _assert { get; }
        public AuthorizeMemberAsOrganizer()
        {
            _assert = new Assertions<NotAuthorizedException<Event>>();
        }

        public void Authorize(string actor, Member oldState, Member newState)
        {
            if (oldState != null && (oldState.IsOrganizer || oldState.IsAuthor))
            {
                _assert.That(oldState.EmailAddress == actor, "Only the Event Author can modify or remove another organizer's info.");
            }

            if (newState != null)
            {
                _assert.That(!newState.IsAuthor, "This property cannot be set by an organizer.");
                _assert.That(!newState.IsOrganizer, "This property cannot be set by an organizer.");
            }
        }
    }
}
