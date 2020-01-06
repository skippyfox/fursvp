using fursvp.helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class AuthorizeMemberAsAuthor : IAuthorize<Member>
    {
        private Assertions<NotAuthorizedException<Event>> _assert { get; }
        public AuthorizeMemberAsAuthor()
        {
            _assert = new Assertions<NotAuthorizedException<Event>>();
        }

        public void Authorize(string actor, Member oldState, Member newState)
        {
        }
    }
}
