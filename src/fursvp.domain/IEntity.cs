// <copyright file="IEntity.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    public interface IEntity<T>
        where T : IEntity<T>
    {
        Guid Id { get; set; }

        int Version { get; set; }
    }
}
