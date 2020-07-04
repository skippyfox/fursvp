// <copyright file="SuppressAndLogEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using System;
    using System.Threading.Tasks;

    public class SuppressAndLogEmailer : IEmailer
    {
        public void Send(Email email)
        {
            Console.WriteLine($@"[{nameof(SuppressAndLogEmailer)} suppressed message] {email.Subject}
From:   {email.From.Address} ({email.From.Name})
To:     {email.To.Address} ({email.To.Name})
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
