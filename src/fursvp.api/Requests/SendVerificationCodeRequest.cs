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
