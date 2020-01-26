// <copyright file="EventController.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fursvp.Api.Requests;
    using Fursvp.Data;
    using Fursvp.Domain;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="logger">The application event logger.</param>
        /// <param name="eventService">The instance of <see cref="IEventService"/> used to create and update Events.</param>
        /// <param name="eventRepository">The instance of <see cref="IRepository{Event}"/> used for Event persistence.</param>
        public EventController(ILogger<EventController> logger, IEventService eventService, IRepository<Event> eventRepository)
        {
            this.Logger = logger;
            this.EventRepository = eventRepository;
            this.EventService = eventService;
        }

        private ILogger<EventController> Logger { get; }

        private IRepository<Event> EventRepository { get; }

        private IEventService EventService { get; }

        [HttpGet]
        public async Task<List<Event>> GetEvents()
        {
            return (await this.EventRepository.GetAll()).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Event> GetEvent(Guid id)
        {
            return this.EventRepository.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] NewMemberRequest author)
        {
            var @event = this.EventService.CreateNewEvent(author.EmailAddress, author.Name);
            await this.EventRepository.Insert(@event);
            return this.CreatedAtAction(nameof(this.GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPut]
        [Route("{eventId}")]
        public async Task<IActionResult> UpdateEvent(Guid eventId, [FromBody] UpdateEventRequest request)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            @event.Location = request.Location;
            @event.Name = request.Name;
            @event.OtherDetails = request.OtherDetails;
            @event.RsvpOpen = request.RsvpOpen;
            @event.RsvpClosesAt = request.RsvpClosesAt;
            @event.StartsAt = request.StartsAt;
            @event.EndsAt = request.EndsAt;

            await this.EventRepository.Update(@event);
            return this.Ok(@event);
        }

        [HttpPost]
        [Route("{eventId}/publish")]
        public async Task<IActionResult> PublishEvent(Guid eventId)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            @event.IsPublished = true;

            await this.EventRepository.Update(@event);
            return this.Ok(@event);
        }

        [HttpDelete]
        [Route("{eventId}/publish")]
        public async Task<IActionResult> UnpublishEvent(Guid eventId)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            @event.IsPublished = true;

            await this.EventRepository.Update(@event);
            return this.Ok(@event);
        }

        [HttpPost]
        [Route("{eventId}/member")]
        public async Task<IActionResult> AddMember(Guid eventId, [FromBody] NewMemberRequest newMember)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            if (@event.Members.Any(m => m.EmailAddress.ToLower() == newMember.EmailAddress.ToLower()))
            {
                return this.BadRequest("Email address already exists in member list.");
            }

            var member = new Member
            {
                IsAttending = true,
                EmailAddress = newMember.EmailAddress,
                Name = newMember.Name,
            };
            this.EventService.AddMember(@event, member);
            await this.EventRepository.Update(@event);
            return this.CreatedAtAction(nameof(this.GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPut]
        [Route("{eventId}/member/{memberId}")]
        public async Task<IActionResult> UpdateMember(Guid eventId, Guid memberId, [FromBody] UpdateMemberRequest updateMember)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            if (!updateMember.IsAttending && !updateMember.IsOrganizer)
            {
                return this.BadRequest("Member must be at least one of these: Attendee, Organizer");
            }

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
            {
                return this.NotFound("Member not found with id " + memberId);
            }

            member.IsAttending = updateMember.IsAttending;
            member.IsOrganizer = updateMember.IsOrganizer;

            await this.EventRepository.Update(@event);
            return this.Ok(@event);
        }

        [HttpDelete]
        [Route("{eventId}/member/{memberId}")]
        public async Task<IActionResult> RemoveMember(Guid eventId, Guid memberId)
        {
            var @event = await this.EventRepository.GetById(eventId);
            if (@event == null)
            {
                return this.NotFound("Event not found with id " + eventId);
            }

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
            {
                return this.NotFound("Member not found with id " + memberId);
            }

            @event.Members.Remove(member);

            await this.EventRepository.Update(@event);
            return this.NoContent();
        }
    }
}
