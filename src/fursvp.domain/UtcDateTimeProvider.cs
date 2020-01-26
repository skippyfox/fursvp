// <copyright file="UtcDateTimeProvider.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    public class UtcDateTimeProvider : IProvideDateTime
    {
        public DateTime Now => DateTime.UtcNow;

        public DateTime Today => DateTime.UtcNow.Date;
    }
}
