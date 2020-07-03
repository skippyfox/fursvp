using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fursvp.Communication
{
    public class SuppressAndLogEmailer : IEmailer
    {
        public void Send(Email email)
        {
            Console.WriteLine($@"{nameof(SuppressAndLogEmailer)} received and suppressed message:
From:   {email.From.Address} ({email.From.Name})
To:     {email.To.Address} ({email.To.Name})
Subj:   {email.Subject}
{email.PlainTextContent}
");
        }

        public Task SendAsync(Email email)
        {
            this.Send(email);
            return Task.CompletedTask;
        }
    }
}
