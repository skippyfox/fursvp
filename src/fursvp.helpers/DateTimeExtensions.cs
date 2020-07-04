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
        /// <param name="dateTime">The local datetime.</param>
        /// <param name="sourceTimeZoneId">The local time zone from which to convert to utc time.</param>
        /// <returns>The converted datetime in utc.</returns>
        public static DateTime ToUtc(this DateTime dateTime, string sourceTimeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(dateTime.Ticks), timeZoneInfo);
        }
    }
}