// <copyright file="ValidateTimeZone.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    using System;

    public class ValidateTimeZone
    {
        public void Validate(string id)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(id);
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
