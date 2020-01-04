using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fursvp.data;
using fursvp.domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fursvp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IRepository<Event> _eventRepository;
        private readonly IEventService _eventService;

        public EventController(ILogger<EventController> logger, IEventService eventService, IRepository<Event> eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _eventService = eventService;
        }

        [HttpGet]
        public List<Event> GetEvents()
        {
            return _eventRepository.GetAll().ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public Event GetEvent(Guid id)
        {
            return _eventRepository.GetById(id);
        }

        [HttpPost]
        public Event CreateEvent(string emailAddress, string name)
        {
            var @event = _eventService.CreateNewEvent(emailAddress, emailAddress, name);
            _eventRepository.Insert(@event);
            return @event;
        }

        [HttpPost]
        [Route("{eventId}/member")]
        public Event AddMember(Guid eventId, string emailAddress, string name)
        {
            var @event = _eventRepository.GetById(eventId);
            var member = new Member {
                EmailAddress = emailAddress,
                Name = name
            };
            _eventService.AddMember(@event, member);
            _eventRepository.Update(@event);
            return @event;
        }
    }
}
