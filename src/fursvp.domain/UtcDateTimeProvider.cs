// <copyright file="UtcDateTimeProvider.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    /// <summary>
    /// Provides commonly requested DateTime values as UTC times.
    /// </summary>
    public class UtcDateTimeProvider : IProvideDateTime
    {
        /// <summary>
        /// Gets DateTime.UtcNow.
        /// </summary>
        public DateTime Now => DateTime.UtcNow;

        /// <summary>
        /// Gets DateTime.UtcNow.Date.
        /// </summary>
        public DateTime Today => DateTime.UtcNow.Date;
    }
}
