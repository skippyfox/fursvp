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
    using System.Web;
    using AutoMapper;
    using Fursvp.Api.Requests;
    using Fursvp.Api.Responses;
    using Fursvp.Communication;
    using Fursvp.Data;
    using Fursvp.Domain;
    using Fursvp.Helpers;
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
        /// <param name="eventRepositoryWrite">The instance of <see cref="IRepositoryWrite{Event}"/> used for Event persistence write operations.</param>
        /// <param name="eventRepositoryRead">The instance of <see cref="IRepositoryRead{Event}"/> used for Event persistence read operations.</param>
        /// <param name="emailer">The emailer what sends the emails.</param>
        public EventController(ILogger<EventController> logger, IEventService eventService, IRepositoryWrite<Event> eventRepositoryWrite, IRepositoryRead<Event> eventRepositoryRead, IEmailer emailer, IMapper mapper)
        {
            Logger = logger;
            EventRepositoryWrite = eventRepositoryWrite;
            EventRepositoryRead = eventRepositoryRead;
            EventService = eventService;
            Emailer = emailer;
            Mapper = mapper;
        }

        private IMapper Mapper { get; }

        private ILogger<EventController> Logger { get; }

        private IRepositoryWrite<Event> EventRepositoryWrite { get; }

        private IRepositoryRead<Event> EventRepositoryRead { get; }

        private IEventService EventService { get; }

        private IEmailer Emailer { get; }

        /// <summary>
        /// Retrieves details for all Events.
        /// </summary>
        /// <returns>A list of objects representing each Event.</returns>
        [HttpGet]
        public async Task<List<EventResponse>> GetEvents()
        {
            var allEvents = await EventRepositoryRead.GetAll().ConfigureAwait(false);

            // TODO: Only get events that are not over yet

            return allEvents.Select(Mapper.MapResponse).ToList();
        }

        /// <summary>
        /// Retrieves details for an Event.
        /// </summary>
        /// <param name="id">The globally unique identifier of the Event to update.</param>
        /// <returns>An object representing the Event matching the id.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var @event = await EventRepositoryRead.GetById(id).ConfigureAwait(false);

            if (@event == null)
            {
                return NotFound("Event not found with id " + id);
            }

            return Ok(Mapper.MapResponse(@event));
        }

        /// <summary>
        /// Creates a new Event.
        /// </summary>
        /// <param name="newEvent">The <see cref="NewEventRequest"/> containing information about the author of the new Event.</param>
        /// <returns>201 Created on success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] NewEventRequest newEvent)
        {
            if (newEvent == null)
            {
                throw new ArgumentNullException(nameof(newEvent));
            }

            var @event = EventService.CreateNewEvent(newEvent.AuthorEmailAddress, newEvent.AuthorName);
            await EventRepositoryWrite.Insert(@event).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, Mapper.MapResponse(@event));
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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            // TODO: Move this into EventService

            @event.Location = request.Location;
            @event.Name = request.Name;
            @event.OtherDetails = request.OtherDetails;
            @event.RsvpOpen = request.RsvpOpen;
            @event.TimeZoneId = request.TimeZoneId;
            @event.Form.Clear();
            foreach (var prompt in request.Form)
            {
                if (prompt.Id == Guid.Empty)
                {
                    prompt.Id = Guid.NewGuid(); //TODO - move to EventService
                }

                @event.Form.Add(prompt);
            }

            if (request.RsvpClosesAtLocal.HasValue)
            {
                request.RsvpClosesAtLocal.Value.TryToUtc(request.TimeZoneId, out var rsvpClosesAtUtc);
                @event.RsvpClosesAtUtc = rsvpClosesAtUtc;
            }
            else
            {
                @event.RsvpClosesAtUtc = null;
            }
            
            request.StartsAtLocal.TryToUtc(request.TimeZoneId, out var startsAtUtc);
            @event.StartsAtUtc = startsAtUtc;
            
            request.EndsAtLocal.TryToUtc(request.TimeZoneId, out var endsAtUtc);
            @event.EndsAtUtc = endsAtUtc;

            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);
            return Ok(Mapper.MapResponse(@event));
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
            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            @event.IsPublished = true;

            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);
            return Ok(Mapper.MapResponse(@event));
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
            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            @event.IsPublished = true;

            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);
            return Ok(Mapper.MapResponse(@event));
        }

        /// <summary>
        /// Adds a new member to an Event's signups.
        /// </summary>
        /// <param name="eventId">The globally unique identifier for the Event.</param>
        /// <param name="newMember">The <see cref="NewMemberRequest" /> containing member data to be added.</param>
        /// <returns>201 Created on success, 404 Not Found if the Event Id is not found, or 409 Conflict if the member's email address already exists in the event.</returns>
        [HttpPost]
        [Route("{eventId}/member")]
        public async Task<IActionResult> AddMember(Guid eventId, [FromBody] NewMemberRequest newMember)
        {
            if (newMember == null)
            {
                throw new ArgumentNullException(nameof(newMember));
            }

            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            var member = new Member
            {
                IsAttending = true,
                EmailAddress = newMember.EmailAddress,
                Name = newMember.Name,
            };

            foreach (var response in newMember.FormResponses) 
            {
                member.Responses.Add(response);
            }

            EventService.AddMember(@event, member);
            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);

            _ = Task.Run(() =>
              {
                // TODO: Get hardcoded strings into config
                Emailer?.Send(new Email
                  {
                      From = new EmailAddress { Address = "noreply@fursvp.com", Name = "Fursvp.com" },
                      To = new EmailAddress { Address = newMember.EmailAddress, Name = newMember.Name },
                      Subject = $"{@event.Name}: Your event registration",
                      PlainTextContent = @$"{newMember.Name}: We've got you on the list! View the event details or review and change your response at Fursvp.com.",
                      HtmlContent = @$"{HttpUtility.HtmlEncode(newMember.Name)}: We've got you on the list! View the event details or review and change your response at <a href=""https://www.fursvp.com"">FURsvp.com</a>.",
                  });
              });

            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, Mapper.MapResponse(@event));
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
            if (updateMember == null)
            {
                throw new ArgumentNullException(nameof(updateMember)); 
            }

            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
            {
                return NotFound("Member not found with id " + memberId);
            }

            member.IsAttending = updateMember.IsAttending;
            member.IsOrganizer = updateMember.IsOrganizer;
            member.EmailAddress = updateMember.EmailAddress;
            member.Name = updateMember.Name;

            member.Responses.Clear();
            foreach (var response in updateMember.FormResponses)
            {
                member.Responses.Add(response);
            }

            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);
            return Ok(Mapper.MapResponse(@event));
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
            var @event = await EventRepositoryRead.GetById(eventId).ConfigureAwait(false);
            if (@event == null)
            {
                return NotFound("Event not found with id " + eventId);
            }

            var member = @event.Members.SingleOrDefault(x => x.Id == memberId);
            if (member == null)
            {
                return NotFound("Member not found with id " + memberId);
            }

            @event.Members.Remove(member);

            await EventRepositoryWrite.Update(@event).ConfigureAwait(false);
            return NoContent();
        }
    }
}
