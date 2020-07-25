// <copyright file="DateTimeExtensions.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Helpers
{
    using System;

    /// <summary>
    /// Extension methods for DateTime objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime from local time zone to utc time.
        /// </summary>
        /// <param name="localDateTime">The local datetime.</param>
        /// <param name="sourceTimeZoneId">The local time zone from which to convert to utc time.</param>
        /// <returns>The converted datetime in utc.</returns>
        public static DateTime ToUtc(this DateTime localDateTime, string sourceTimeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
            return ToUtc(localDateTime, timeZoneInfo);
        }

        /// <summary>
        /// Converts a DateTime from local time zone to utc time.
        /// </summary>
        /// <param name="localDateTime">The local datetime.</param>
        /// <param name="sourceTimeZoneInfo">The local time zone from which to convert to utc time.</param>
        /// <returns>The converted datetime in utc.</returns>
        public static DateTime ToUtc(this DateTime localDateTime, TimeZoneInfo sourceTimeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(localDateTime.Ticks), sourceTimeZoneInfo);
        }

        /// <summary>
        /// Converts a DateTime from local time zone to utc time.
        /// </summary>
        /// <param name="localDateTime">The local datetime.</param>
        /// <param name="sourceTimeZoneId">The local time zone from which to convert to utc time.</param>
        /// <param name="utcDateTime">The utc datetime result, or the local datetime if conversion was unsuccessful.</param>
        /// <returns>A value indicating whether the conversion was successful.</returns>
        public static bool TryConvertToUtc(this DateTime localDateTime, string sourceTimeZoneId, out DateTime utcDateTime)
        {
            try
            {
                utcDateTime = localDateTime.ToUtc(sourceTimeZoneId);
                return true;
            }
            catch
            {
                utcDateTime = localDateTime;
                return false;
            }
        }

        /// <summary>
        /// Converts a DateTime from utc time to a local time zone.
        /// </summary>
        /// <param name="utcDateTime">The utc datetime.</param>
        /// <param name="targetTimeZoneId">The local time zone to which to convert from utc time.</param>
        /// <returns>The local datetime.</returns>
        public static DateTime ToLocal(this DateTime utcDateTime, string targetTimeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneId);
            return ToLocal(utcDateTime, timeZoneInfo);
        }

        /// <summary>
        /// Converts a DateTime from utc time to a local time zone.
        /// </summary>
        /// <param name="utcDateTime">The utc datetime.</param>
        /// <param name="targetTimeZoneInfo">The local time zone to which to convert from utc time.</param>
        /// <returns>The local datetime.</returns>
        public static DateTime ToLocal(this DateTime utcDateTime, TimeZoneInfo targetTimeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(utcDateTime.Ticks), targetTimeZoneInfo);
        }

        /// <summary>
        /// Converts a DateTime from utc time to a local time zone.
        /// </summary>
        /// <param name="utcDateTime">The utc datetime.</param>
        /// <param name="targetTimeZoneId">The local time zone to which to convert from utc time.</param>
        /// <param name="localDateTime">The local datetime result, or the utc datetime if conversion was unsuccessful.</param>
        /// <returns>A value indicating whether the conversion was successful.</returns>
        public static bool TryToLocal(this DateTime utcDateTime, string targetTimeZoneId, out DateTime localDateTime)
        {
            try
            {
                localDateTime = utcDateTime.ToLocal(targetTimeZoneId);
                return true;
            }
            catch
            {
                localDateTime = utcDateTime;
                return false;
            }
        }
    }
}