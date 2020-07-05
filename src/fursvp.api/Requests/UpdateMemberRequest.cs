// <copyright file="UpdateMemberRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using Fursvp.Domain.Forms;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The request model pertaining to inserting or updating Member info.
    /// </summary>
    public class UpdateMemberRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether an event member is attending the event.
        /// </summary>
        [Required]
        public bool IsAttending { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an event member is an organizer.
        /// </summary>
        [Required]
        public bool IsOrganizer { get; set; }

        /// <summary>
        /// Gets or sets the Email Address for the member.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the display name for the member.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of Form Responses for the event sign-up form.
        /// </summary>
        [Required]
        public IEnumerable<FormResponses> FormResponses { get; set; }
    }
}
