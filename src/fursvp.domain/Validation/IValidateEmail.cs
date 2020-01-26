// <copyright file="IValidateEmail.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    public interface IValidateEmail
    {
        void Validate(string address);
    }
}
