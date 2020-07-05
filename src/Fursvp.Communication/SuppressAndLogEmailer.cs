// <copyright file="SuppressAndLogEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Suppresses and logs emails instead of emailing them.
    /// </summary>
    public class SuppressAndLogEmailer : IEmailer
    {
        /// <summary>
        /// Suppresses an email and logs it instead.
        /// </summary>
        /// <param name="email">The email to suppress and log.</param>
        public void Send(Email email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            Console.WriteLine($@"[{nameof(SuppressAndLogEmailer)} suppressed message] {email.Subject}
From:   {email.From.Address} ({email.From.Name})
To:     {email.To.Address} ({email.To.Name})
{email.PlainTextContent}
");
        }

        /// <summary>
        /// Suppresses an email and logs it instead, then returns a completed task.
        /// </summary>
        /// <param name="email">The email to suppress and log.</param>
        /// <returns>An object representing the a completed task operation.</returns>
        public Task SendAsync(Email email)
        {
            Send(email);
            return Task.CompletedTask;
        }
    }
}
