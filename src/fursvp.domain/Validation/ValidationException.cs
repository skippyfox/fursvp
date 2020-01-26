// <copyright file="ValidationException.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;

    /// <summary>
    /// Thrown when the attempted transition between two states of the same type is not valid. For use with Domain validation, not endpoint request validation.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type of which the compared states are invalid.</param>
        public ValidationException(string message, Type type)
            : base(message)
        {
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type of which the compared states are invalid.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ValidationException(string message, Type type, Exception innerException)
            : base(message, innerException)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the type of which the compared states are invalid.
        /// </summary>
        public Type Type { get; }
    }

    /// <summary>
    /// Thrown when the attempted transition between two states (instances of type T) is not valid. For use with Domain validation, not endpoint request validation.
    /// </summary>
    /// <typeparam name="T">The type of which the compared states are invalid.</typeparam>
    public class ValidationException<T> : ValidationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationException(string message)
            : base(message, typeof(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ValidationException(string message, Exception innerException)
            : base(message, typeof(T), innerException)
        {
        }
    }
}
