// <copyright file="User.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    /// <summary>
    /// Provides information about a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string EmailAddress { get; set; }
        public string SessionId { get; set; }
    }
}
