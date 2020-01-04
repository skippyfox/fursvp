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
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private ILogger<EventController> _logger { get; }
        private IRepository<Event> _eventRepository { get; }
        private IEventService _eventService { get; }

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
        public IActionResult CreateEvent([FromBody] Member organizer)
        {
            var @event = _eventService.CreateNewEvent(organizer.EmailAddress, organizer.EmailAddress, organizer.Name);
            _eventRepository.Insert(@event);
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPost]
        [Route("{eventId}/member")]
        public IActionResult AddMember(Guid eventId, [FromBody] Member member)
        {
            var @event = _eventRepository.GetById(eventId);
            member.IsAttending = true;
            _eventService.AddMember(@event, member);
            _eventRepository.Update(@event);
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }
    }
}
