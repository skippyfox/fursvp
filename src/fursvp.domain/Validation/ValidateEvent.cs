using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.domain.Validation
{
    public class ValidateEvent : IValidateEntity<Event>
    {
        private IProvideDateTime _dateTimeProvider { get; }
        public ValidateEvent(IProvideDateTime dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void ValidateDelete(Event entity)
        {
            throw new ValidationException<Event>("Deleting an Event is not allowed.");
        }

        public void ValidateState(Event oldState, Event newState)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            if (newState.Members == null)
                throw new ValidationException<Event>("Members list cannot be empty or null.");

            if (!newState.Members.Any(x => x.IsAuthor))
                throw new ValidationException<Event>("Members list must contain the author of the event.");

            if (newState.Form == null)
                throw new ValidationException<Event>("Form cannot be null.");

            if (!newState.RsvpOpen && newState.RsvpClosesAt.HasValue)
                throw new ValidationException<Event>("Rsvp cannot be closed while RsvpClosesAt is set.");

            if (newState.RsvpOpen && !newState.RsvpClosesAt.HasValue)
                throw new ValidationException<Event>("RsvpClosesAt must be set if Rsvp is open.");
            
            if (newState.IsPublished)
            {
                if (newState.StartsAt == default)
                    throw new ValidationException<Event>("Event must have a start date and time.");

                if (newState.EndsAt == default)
                    throw new ValidationException<Event>("Event must have an end date and time.");

                if (newState.StartsAt > newState.EndsAt)
                    throw new ValidationException<Event>("Start time must be before End time.");

                if (oldState?.IsPublished != true)
                {
                    if (newState.StartsAt < _dateTimeProvider.Now)
                        throw new ValidationException<Event>("Start time cannot be in the past.");
                }

                if (newState.TimeZone == null)
                    throw new ValidationException<Event>("Time Zone is required.");
            }
        }
    }
}
