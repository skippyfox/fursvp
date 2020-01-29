// <copyright file="IEntity.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System;

    /// <summary>
    /// A persistable domain entity or root aggregate that can be identified by a Global Unique Identifier and an integer Version.
    /// </summary>
    /// <typeparam name="T">The domain entity type.</typeparam>
    public interface IEntity<T>
        where T : IEntity<T>
    {
        /// <summary>
        /// Gets or sets the global unique identifier for a persistable instance of the entity.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Version of this entity used for database version control.
        /// </summary>
        int Version { get; set; }
    }
}
