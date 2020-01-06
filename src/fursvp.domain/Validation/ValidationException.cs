using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Validation
{
    /// <summary>
    /// For use with Domain validation, not endpoint request validation
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public ValidationException(string message, Type type, Exception innerException) : base(message, innerException)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    /// <summary>
    /// For use with Domain validation, not endpoint request validation
    /// </summary>
    public class ValidationException<T> : ValidationException
    {
        public ValidationException(string message) : base(message, typeof(T))
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, typeof(T), innerException)
        {
        }
    }
}
