using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fursvp.helpers;

namespace fursvp.domain.Validation
{
    public class ValidateMember : IValidate<Member>
    {
        private IValidateEmail _validateEmail { get; }
        private Assertions<ValidationException<Member>> _assert { get; }
        public ValidateMember(IValidateEmail validateEmail)
        {
            _validateEmail = validateEmail;
            _assert = new Assertions<ValidationException<Member>>();
        }

        public void ValidateState(Member oldState, Member newState)
        {
            if (oldState == null && newState == null)
                throw new ArgumentNullException(nameof(newState));
            
            if (newState == null)
                return; // Delete member

            _assert.That(!string.IsNullOrWhiteSpace(newState.EmailAddress), "Email address is required.");
            _validateEmail.Validate(newState.EmailAddress);
            _assert.That(!string.IsNullOrWhiteSpace(newState.Name), "Name is required.");
            _assert.That(newState.IsAuthor || newState.IsOrganizer || newState.IsAttending, "Member must be the Author, an Organizer, or Attending.");
            _assert.That(newState.Responses != null, "Responses cannot be null.");
        }
    }
}
