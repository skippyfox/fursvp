// <copyright file="EmailAddress.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    /// <summary>
    /// Represents the name and email address of a sender or recipient.
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// Gets or sets the name of the sender or recipient.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the sender or recipient.
        /// </summary>
        public string Address { get; set; }
    }
}
