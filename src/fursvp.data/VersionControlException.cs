using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.data
{
    public class VersionControlException : Exception
    {
        public VersionControlException(string message, Type type) : base(message)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    public class VersionControlException<T> : VersionControlException
    {
        public VersionControlException(string message) : base(message, typeof(T))
        {
        }
    }
}
