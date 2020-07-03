using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Fursvp.Communication
{
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
            var from = new SendGrid.Helpers.Mail.EmailAddress(email.From.Address, email.From.Name);
            var to = new SendGrid.Helpers.Mail.EmailAddress(email.To.Address, email.To.Name);
            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContent, email.HtmlContent);
            var response = await client.SendEmailAsync(sendGridMessage).ConfigureAwait(false);
        }
    }
}
