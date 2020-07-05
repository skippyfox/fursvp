// <copyright file="IRepository.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using Fursvp.Domain;

    /// <summary>
    /// Provides create/update/delete persistence logic for a domain entity.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity{T}"/> type.</typeparam>
    public interface IRepository<T> : IRepositoryRead<T>, IRepositoryWrite<T>
        where T : IEntity<T>
    {
    }
}
