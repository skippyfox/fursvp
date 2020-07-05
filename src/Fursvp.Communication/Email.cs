// <copyright file="Email.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    /// <summary>
    /// Represents a single copy of an email.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Gets or sets the sender's name and address.
        /// </summary>
        public EmailAddress From { get; set; }

        /// <summary>
        /// Gets or sets the recipient's name and address.
        /// </summary>
        public EmailAddress To { get; set; }

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the plain text content of the email.
        /// </summary>
        public string PlainTextContent { get; set; }

        /// <summary>
        /// Gets or sets the HTML-formatted content of the email.
        /// </summary>
        public string HtmlContent { get; set; }
    }
}
