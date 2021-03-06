﻿// <copyright file="VersionConflictException.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;


    /// <summary>
    /// An exception that is thrown when persistence logic encounters a Version conflict.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These exception types have nonstandard constructors because the type parameter is required.")]
    public class VersionConflictException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConflictException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="type">The type of which the compared versions do not match.</param>
        public VersionConflictException(string message, Type type)
            : base(message)
        {
            SourceType = type;
        }

        /// <summary>
        /// Gets the context of the exception.
        /// </summary>
        public Type SourceType { get; }
    }


    /// <summary>
    /// An exception that is thrown when persistence logic encounters a Version conflict.
    /// </summary>
    /// <typeparam name="T">The context of the exception.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These exception types have nonstandard constructors because the type parameter is required.")]
    public class VersionConflictException<T> : VersionConflictException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConflictException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VersionConflictException(string message)
            : base(message, typeof(T))
        {
        }
    }
}
