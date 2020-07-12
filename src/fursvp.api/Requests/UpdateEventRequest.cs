// <copyright file="UpdateEventRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using Fursvp.Domain.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The request model representing an updated Event.
    /// </summary>
    public class UpdateEventRequest
    {
        /// <summary>
        /// Gets or sets the locaiton of the Event.
        /// </summary>
        [Required]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Event Name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets miscellaneous information about an Event.
        /// </summary>
        public string OtherDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RSVPs for an Event are open before RsvpClosesAt.
        /// </summary>
        [Required]
        public bool RsvpOpen { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which RSVPs for the Event are scheduled to close.
        /// </summary>
        public DateTime? RsvpClosesAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the Event is scheduled to start.
        /// </summary>
        [Required]
        public DateTime StartsAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the Event is scheduled to end.
        /// </summary>
        [Required]
        public DateTime EndsAt { get; set; }

        /// <summary>
        /// Gets or sets the time zone id for the event.
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets form prompts and options for an Event.
        /// </summary>
        public ICollection<FormPrompt> Form { get; set; }
    }
}
