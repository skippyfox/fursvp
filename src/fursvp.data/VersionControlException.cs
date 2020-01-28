// <copyright file="VersionControlException.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;

    /// <summary>
    /// An exception that is thrown when persistence logic encounters a Version conflict.
    /// </summary>
    public class VersionControlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionControlException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type of which the compared versions do not match.</param>
        public VersionControlException(string message, Type type)
            : base(message)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the context of the exception.
        /// </summary>
        public Type Type { get; }
    }

    /// <summary>
    /// An exception that is thrown when persistence logic encounters a Version conflict.
    /// </summary>
    /// <typeparam name="T">The context of the exception.</typeparam>
    public class VersionControlException<T> : VersionControlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionControlException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VersionControlException(string message)
            : base(message, typeof(T))
        {
        }
    }
}
