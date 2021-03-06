﻿// <copyright file="NotAuthorizedException.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    using System;


    /// <summary>
    /// An Exception that represents an unauthorized attempt at an action.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These exception types have nonstandard constructors because the type parameter is required.")]
    public class NotAuthorizedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type for which the change is not authorized.</param>
        public NotAuthorizedException(string message, Type type)
            : base(message)
        {
            SourceType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type for which the change is not authorized.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public NotAuthorizedException(string message, Type type, Exception innerException)
            : base(message, innerException)
        {
            SourceType = type;
        }

        /// <summary>
        /// Gets the type for which the change is not authorized.
        /// </summary>
        public Type SourceType { get; }
    }


    /// <summary>
    /// An Exception that represents an unauthorized attempt at an action.
    /// </summary>
    /// <typeparam name="T">The type against which the action is not authorized.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These exception types have nonstandard constructors because the type parameter is required.")]
    public class NotAuthorizedException<T> : NotAuthorizedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotAuthorizedException(string message)
            : base(message, typeof(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public NotAuthorizedException(string message, Exception innerException)
            : base(message, typeof(T), innerException)
        {
        }
    }
}
