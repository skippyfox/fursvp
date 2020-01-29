// <copyright file="IValidateEmail.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    /// <summary>
    /// Provides logic to ensure that an email address string is valid.
    /// </summary>
    public interface IValidateEmail
    {
        /// <summary>
        /// Throws an Exception if the email address string is not considered valid.
        /// </summary>
        /// <param name="address">The email address.</param>
        void Validate(string address);
    }
}
