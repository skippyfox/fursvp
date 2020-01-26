// <copyright file="ValidateEmail.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;
    using System.Net.Mail;

    public class ValidateEmail : IValidateEmail
    {
        public void Validate(string address)
        {
            try
            {
                new MailAddress(address);
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
