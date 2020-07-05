// <copyright file="VerifyEmailRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The request model representing the email and verification code for email verification.
    /// </summary>
    public class VerifyEmailRequest
    {
        /// <summary>
        /// Gets or sets the Email Address to verify.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the verification code.
        /// </summary>
        [Required]
        public string VerificationCode { get; set; }
    }
}
