using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain
{
    public class EventService : IEventService
    {
        public Event CreateNewEvent(string authenticatedEmailAddress, string emailAddress, string name)
        {
            if (authenticatedEmailAddress != emailAddress)
                throw new UnauthenticatedUserException();

            var author = new Member
            {
                EmailAddress = emailAddress,
                IsAuthor = true,
                IsOrganizer = true,
                IsAttending = true,
                Name = name
            };

            var @event = new Event()
            {
                Id = Guid.NewGuid(),
                Members = new List<Member> { author }
            };

            return @event;
        }

        public void AddMember(Event @event, Member member)
        {
            @event.Members.Add(member);
        }

        //TODO: Update with authentication check

        //TODO: Update/Remove Members
    }
}
