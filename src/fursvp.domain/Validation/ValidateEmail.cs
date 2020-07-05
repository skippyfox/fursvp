// <copyright file="ValidateEmail.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;
    using System.Net.Mail;

    /// <summary>
    /// Provides logic to ensure that an email address string is valid based on <see cref="MailAddress"/> constructor validation.
    /// </summary>
    public class ValidateEmail : IValidateEmail
    {
        /// <summary>
        /// Throws an Exception if the email address string is not considered valid based on <see cref="MailAddress"/> constructor validation.
        /// </summary>
        /// <param name="address">The email address.</param>
        public void Validate(string address)
        {
            try
            {
                // We're not using this MailAddress - we just need to try and create it and see if an exception is thrown.
                _ = new MailAddress(address);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidationException<string>(ex.Message, ex);
            }
        }
    }
}
