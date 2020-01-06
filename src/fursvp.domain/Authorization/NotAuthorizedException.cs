using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Authorization
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public NotAuthorizedException(string message, Type type, Exception innerException) : base(message, innerException)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    public class NotAuthorizedException<T> : NotAuthorizedException
    {
        public NotAuthorizedException(string message) : base(message, typeof(T))
        {
        }

        public NotAuthorizedException(string message, Exception innerException) : base(message, typeof(T), innerException)
        {
        }
    }
}
