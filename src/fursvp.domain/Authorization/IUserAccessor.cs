// <copyright file="IUserAccessor.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    /// <summary>
    /// Accesses an <see cref="User" /> instance.
    /// </summary>
    public interface IUserAccessor
    {
        /// <summary>
        /// Gets the <see cref="User"/>.
        /// </summary>
        User User { get; }
    }
}
