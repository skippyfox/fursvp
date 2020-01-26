// <copyright file="UpdateEventRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateEventRequest
    {
        [Required]
        public string Location { get; set; }

        [Required]
        public string Name { get; set; }

        public string OtherDetails { get; set; }

        [Required]
        public bool RsvpOpen { get; set; }

        public DateTime RsvpClosesAt { get; set; }

        [Required]
        public DateTime StartsAt { get; set; }

        [Required]
        public DateTime EndsAt { get; set; }
    }
}
