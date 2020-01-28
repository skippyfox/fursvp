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

    /// <summary>
    /// Manages Events.
    /// </summary>
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

        /// <summary>
        /// Retrieves details for all Events.
        /// </summary>
        /// <returns>A list of objects representing each Event.</returns>
        [HttpGet]
        public async Task<List<Event>> GetEvents()
        {
            return (await this.EventRepository.GetAll()).ToList();
        }

        /// <summary>
        /// Retrieves details for an Event.
        /// </summary>
        /// <param name="id">The globally unique identifier of the Event to update.</param>
        /// <returns>An object representing the Event matching the id.</returns>
        [HttpGet]
        [Route("{id}")]
        public Task<Event> GetEvent(Guid id)
        {
            return this.EventRepository.GetById(id);
        }

        /// <summary>
        /// Creates a new Event.
        /// </summary>
        /// <param name="author">The <see cref="NewMemberRequest"/> for the author of the Event.</param>
        /// <returns>201 Created on success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] NewMemberRequest author)
        {
            var @event = this.EventService.CreateNewEvent(author.EmailAddress, author.Name);
            await this.EventRepository.Insert(@event);
            return this.CreatedAtAction(nameof(this.GetEvent), new { id = @event.Id }, @event);
        }

        /// <summary>
        /// Updates an Event.
        /// </summary>
        /// <param name="eventId">The globally unique identifier of the Event to update.</param>
        /// <param name="request">The <see cref="UpdateEventRequest" /> containing new Event details.</param>
        /// <returns>200 Ok on success, or 404 Not Found if the Event Id is not found.</returns>
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

        /// <summary>
        /// Updates an Event so that it is publicly visible.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <returns>200 Ok on success, or 404 Not Found if the Event Id is not found.</returns>
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

        /// <summary>
        /// Updates an Event so that it is no longer publicly visible.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <returns>200 Ok on success, or 404 Not Found if the Event Id is not found.</returns>
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

        /// <summary>
        /// Adds a new member to an Event's signups.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <param name="newMember">The <see cref="NewMemberRequest" /> containing member data to be added.</param>
        /// <returns>201 Created on success, 404 Not Found if the Event Id is not found, or 400 Bad Request if the member's email address already exists in the event.</returns>
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

        /// <summary>
        /// Updates the member's info for an Event.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <param name="memberId">The globally unique identifier for the Event's Member to be updated.</param>
        /// <param name="updateMember">The <see cref="UpdateMemberRequest" /> containing member data to be updated.</param>
        /// <returns>200 Ok on success, 404 Not Found if either the Event Id or Member Id are not found, or 400 Bad Request if the member is not either attending or an organizer.</returns>
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

        /// <summary>
        /// Removes a Member from an Event's sign-ups.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <param name="memberId">The globally unique identifier for the Event's Member to be removed.</param>
        /// <returns>204 No Content on success, or 404 Not Found if either the Event Id or Member Id are not found.</returns>
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
