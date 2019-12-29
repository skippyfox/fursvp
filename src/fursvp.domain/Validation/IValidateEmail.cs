using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Validation
{
    public interface IValidateEmail
    {
        void Validate(string emailAddress);
    }
}
