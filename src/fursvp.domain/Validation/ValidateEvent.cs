using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fursvp.helpers;

namespace fursvp.domain.Validation
{
    public class ValidateEvent : IValidate<Event>
    {
        private IProvideDateTime _dateTimeProvider { get; }
        private IValidate<Member> _validateMember { get; }
        private Assertions<ValidationException<Event>> _assert { get; }
        public ValidateEvent(IProvideDateTime dateTimeProvider, IValidate<Member> validateMember)
        {
            _dateTimeProvider = dateTimeProvider;
            _validateMember = validateMember;
            _assert = new Assertions<ValidationException<Event>>();
        }

        public void ValidateState(Event oldState, Event newState)
        {
            if (oldState == null && newState == null)
                throw new ArgumentNullException(nameof(newState));

            _assert.That(newState != null, "Deleting an Event is not allowed.");
            _assert.That(newState.Members?.Any() == true, "Members list cannot be empty or null.");
            _assert.That(newState.Members.Select(x => x.EmailAddress.ToLower()).Distinct().Count() == newState.Members.Count(), "Each member in list must have a unique email address.");
            _assert.That(newState.Members.Count(x => x.IsAuthor) == 1, "Members list must contain exactly one author of the event.");

            var oldMembers = oldState?.Members ?? Enumerable.Empty<Member>();
            var newMembers = newState?.Members ?? Enumerable.Empty<Member>();
            foreach (var memberState in oldMembers.FullJoin(newMembers, m => m.Id, m => m.Id, (old, @new) => new { old, @new }))
            {
                _validateMember.ValidateState(memberState.old, memberState.@new);
            }

            _assert.That(newState.Form != null, "Form cannot be null.");
            if (!newState.RsvpOpen) _assert.That(!newState.RsvpClosesAt.HasValue, "RsvpClosesAt cannot be set if Rsvp is closed.");
            if (newState.RsvpOpen) _assert.That(newState.RsvpClosesAt.HasValue, "RsvpClosesAt must be set if Rsvp is open.");
            
            if (newState.IsPublished)
            {
                _assert.That(newState.StartsAt != default, "Event must have a start date and time.");
                _assert.That(newState.EndsAt != default, "Event must have an end date and time.");
                _assert.That(newState.StartsAt < newState.EndsAt, "Start time must be before End time.");

                if (oldState?.IsPublished != true)
                {
                    _assert.That(newState.StartsAt > _dateTimeProvider.Now, "Start time must be in the future.");
                }

                _assert.That(newState.TimeZoneId != null, "Time Zone is required.");
            }
        }
    }
}
