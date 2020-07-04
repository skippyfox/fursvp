// <copyright file="SendVerificationCodeRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class SendVerificationCodeRequest
    {
        /// <summary>
        /// Gets or sets the Email Address to verify.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
