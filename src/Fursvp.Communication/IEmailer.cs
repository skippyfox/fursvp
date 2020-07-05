// <copyright file="IEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using System.Threading.Tasks;

    /// <summary>
    /// Sends emails.
    /// </summary>
    public interface IEmailer
    {
        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="email">The email to send.</param>
        void Send(Email email);

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="email">The email to send.</param>
        /// <returns>An object representing the asynchronous task operation.</returns>
        Task SendAsync(Email email);
    }
}
