// <copyright file="SendGridEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System.Threading.Tasks;

    public class SendGridEmailer : IEmailer
    {
        private readonly SendGridOptions _options;

        public SendGridEmailer(IOptions<SendGridOptions> options)
        {
            _options = options.Value;
        }

        public void Send(Email email) => SendAsync(email).GetAwaiter().GetResult();

        public async Task SendAsync(Email email)
        {
            var client = new SendGridClient(_options.ApiKey);
            var sendGridMessage = ConvertFrom(email);
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
