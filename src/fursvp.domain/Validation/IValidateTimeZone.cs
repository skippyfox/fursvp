// <copyright file="IValidateTimeZone.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    /// <summary>
    /// Provides logic to ensure that a TimeZoneId string can be resolved to an instance of TimeZoneInfo.
    /// </summary>
    public interface IValidateTimeZone
    {
        /// <summary>
        /// Throws an exception if a TimeZoneId string cannot be resolved to an instance of TimeZoneInfo.
        /// </summary>
        /// <param name="id">The TimeZoneId.</param>
        void Validate(string id);
    }
}
