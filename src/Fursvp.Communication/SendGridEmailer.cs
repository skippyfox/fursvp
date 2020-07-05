// <copyright file="SendGridEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    /// <summary>
    /// Sends emails using the SendGrid API.
    /// </summary>
    public class SendGridEmailer : IEmailer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridEmailer"/> class.
        /// </summary>
        /// <param name="options">The SendGrid configuration options.</param>
        public SendGridEmailer(IOptions<SendGridOptions> options)
        {
            this.Options = options.Value;
        }

        private SendGridOptions Options { get; }

        /// <summary>
        /// Sends an email synchronously via the SendGrid API.
        /// </summary>
        /// <param name="email">The email to send.</param>
        public void Send(Email email) => this.SendAsync(email).GetAwaiter().GetResult();

        /// <summary>
        /// Sends an email asynchronously via the SendGrid API.
        /// </summary>
        /// <param name="email">The email to send.</param>
        /// <returns>An object representing the asynchronous task operation.</returns>
        public async Task SendAsync(Email email)
        {
            var client = new SendGridClient(this.Options.ApiKey);
            var sendGridMessage = this.ConvertFrom(email);
            var response = await client.SendEmailAsync(sendGridMessage).ConfigureAwait(false);
        }

        private SendGridMessage ConvertFrom(Email email)
        {
            var from = new SendGrid.Helpers.Mail.EmailAddress(email.From.Address, email.From.Name);
            var to = new SendGrid.Helpers.Mail.EmailAddress(email.To.Address, email.To.Name);
            return MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContent, email.HtmlContent);
        }
    }
}
