// <copyright file="IProvideDateTime.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    /// <summary>
    /// Provides commonly requested DateTime values.
    /// </summary>
    public interface IProvideDateTime
    {
        /// <summary>
        /// Gets the current DateTime.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the Date for the current DateTime.
        /// </summary>
        DateTime Today { get; }
    }
}
