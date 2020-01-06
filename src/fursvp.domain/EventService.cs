using fursvp.domain.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain
{
    public class EventService : IEventService
    {
        private IProvideDateTime _dateTimeProvider { get; }
        public EventService(IProvideDateTime dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Event CreateNewEvent(string emailAddress, string name)
        {
            var author = new Member
            {
                Id = Guid.NewGuid(),
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
            member.Id = Guid.NewGuid();
            @event.Members.Add(member);
        }

        public bool RsvpOpen(Event @event)
        {
            return @event.RsvpOpen && _dateTimeProvider.Now < @event.RsvpClosesAt;
        }

        //TODO: Update with authentication check

        //TODO: Update/Remove Members
    }
}
