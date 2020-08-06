// <copyright file="NewEventRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The request model representing a new Member for an Event.
    /// </summary>
    public class NewEventRequest
    {
        /// <summary>
        /// Gets or sets the display name for the author.
        /// </summary>
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the time zone name.
        /// </summary>
        public string TimeZoneId { get; set; }
    }
}
