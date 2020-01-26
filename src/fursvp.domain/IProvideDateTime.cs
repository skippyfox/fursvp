// <copyright file="IProvideDateTime.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    public interface IProvideDateTime
    {
        DateTime Now { get; }

        DateTime Today { get; }
    }
}
