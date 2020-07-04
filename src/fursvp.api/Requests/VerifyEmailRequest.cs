﻿namespace Fursvp.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

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