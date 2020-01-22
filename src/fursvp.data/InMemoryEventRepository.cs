using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fursvp.data
{
    public class InMemoryEventRepository : IRepository<Event>
    {
        private List<Event> _events { get; } = new List<Event>();

        public Task Delete(Guid guid)
        {
            _events.RemoveAll(e => e.Id == guid);
            return Task.CompletedTask;
        }

        private Event DeepCopy(Event @event) 
        {
            return new Event
            {
                Id = @event.Id,
                Version = @event.Version,
                StartsAt = @event.StartsAt,
                EndsAt = @event.EndsAt,
                TimeZoneId = @event.TimeZoneId,
                Members = @event.Members.Select(member => new Member
                {
                    Id = member.Id,
                    EmailAddress = member.EmailAddress,
                    Name = member.Name,
                    IsAttending = member.IsAttending,
                    IsOrganizer = member.IsOrganizer,
                    IsAuthor = member.IsAuthor,
                    Responses = member.Responses.Select(r => new FormResponses
                    {
                        Prompt = r.Prompt,
                        Responses = r.Responses.ToList()
                    }).ToList()
                }).ToList(),
                Form = new List<FormPrompt>(), //We don't have a concrete implementation yet.
                Name = @event.Name,
                OtherDetails = @event.OtherDetails,
                Location = @event.Location,
                RsvpOpen = @event.RsvpOpen,
                RsvpClosesAt = @event.RsvpClosesAt,
                IsPublished = @event.IsPublished
            };
        }
        
        public Task<IQueryable<Event>> GetAll()
        {
            return Task.FromResult(_events.Select(DeepCopy).AsQueryable());
        }

        public Task<Event> GetById(Guid guid)
        {
            return Task.FromResult(DeepCopy(_events.FirstOrDefault(e => e.Id == guid)));
        }

        public Task Insert(Event entity)
        {
            _events.Add(entity);
            return Task.CompletedTask;
        }

        public Task Update(Event entity)
        {
            _events.RemoveAll(e => e.Id == entity.Id);
            Insert(entity);
            return Task.CompletedTask;
        }
    }
}
