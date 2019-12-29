using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    public class ValidationException<T> : ValidationException
    {
        public ValidationException(string message) : base(message, typeof(T))
        {
        }
    }
}
