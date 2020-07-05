// <copyright file="SendGridOptions.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    /// <summary>
    /// A collection of configuration options for use with the <see cref="SendGridEmailer"/>.
    /// </summary>
    public class SendGridOptions
    {
        /// <summary>
        /// The value "SendGrid".
        /// </summary>
        public const string SendGrid = "SendGrid";

        /// <summary>
        /// Gets or sets the SendGrid API key.
        /// </summary>
        public string ApiKey { get; set; }
    }
}
