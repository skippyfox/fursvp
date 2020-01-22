using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fursvp.api.Requests;
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
        public async Task<List<Event>> GetEvents()
        {
            return (await _eventRepository.GetAll()).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Event> GetEvent(Guid id)
        {
            return _eventRepository.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] NewMemberRequest author)
        {
            var @event = _eventService.CreateNewEvent(author.EmailAddress, author.Name);
            await _eventRepository.Insert(@event);
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPut]
        [Route("{eventId}")]
        public async Task<IActionResult> UpdateEvent(Guid eventId, [FromBody] UpdateEventRequest request)
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);

            @event.Location = request.Location;
            @event.Name = request.Name;
            @event.OtherDetails = request.OtherDetails;
            @event.RsvpOpen = request.RsvpOpen;
            @event.RsvpClosesAt = request.RsvpClosesAt;
            @event.StartsAt = request.StartsAt;
            @event.EndsAt = request.EndsAt;

            await _eventRepository.Update(@event);
            return Ok(@event);
        }

        [HttpPost]
        [Route("{eventId}/publish")]
        public async Task<IActionResult> PublishEvent(Guid eventId)
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);

            @event.IsPublished = true;

            await _eventRepository.Update(@event);
            return Ok(@event);
        }

        [HttpDelete]
        [Route("{eventId}/publish")]
        public async Task<IActionResult> UnpublishEvent(Guid eventId)
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);

            @event.IsPublished = true;

            await _eventRepository.Update(@event);
            return Ok(@event);
        }

        [HttpPost]
        [Route("{eventId}/member")]
        public async Task<IActionResult> AddMember(Guid eventId, [FromBody] NewMemberRequest newMember)
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);
            if (@event.Members.Any(m => m.EmailAddress.ToLower() == newMember.EmailAddress.ToLower()))
                return BadRequest("Email address already exists in member list.");

            var member = new Member
            {
                IsAttending = true,
                EmailAddress = newMember.EmailAddress,
                Name = newMember.Name
            };
            _eventService.AddMember(@event, member);
            await _eventRepository.Update(@event);
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPut]
        [Route("{eventId}/member/{memberId}")]
        public async Task<IActionResult> UpdateMember(Guid eventId, Guid memberId, [FromBody] UpdateMemberRequest updateMember) 
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);
            if (!updateMember.IsAttending && !updateMember.IsOrganizer)
                return BadRequest("Member must be at least one of these: Attendee, Organizer");

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
                return NotFound("Member not found with id " + memberId);

            member.IsAttending = updateMember.IsAttending;
            member.IsOrganizer = updateMember.IsOrganizer;

            await _eventRepository.Update(@event);
            return Ok(@event);
        }

        [HttpDelete]
        [Route("{eventId}/member/{memberId}")]
        public async Task<IActionResult> RemoveMember(Guid eventId, Guid memberId)
        {
            var @event = await _eventRepository.GetById(eventId);
            if (@event == null)
                return NotFound("Event not found with id " + eventId);

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
                return NotFound("Member not found with id " + memberId);

            @event.Members.Remove(member);

            await _eventRepository.Update(@event);
            return NoContent();
        }
    }
}
