// <copyright file="IReadAuthorize.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    /// <summary>
    /// Checks for authorization by a given actor to perform a change to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type against which an attempted action must be authorized.</typeparam>
    public interface IReadAuthorize<T>
    {
        /// <summary>
        /// Performs an authorization check for a read.
        /// </summary>
        /// <param name="content">The content being read.</param>
        /// <returns>True if the user is authorized to view the object.</returns>
        bool CanRead(T content);

        /// <summary>
        /// Changes any part of the object contents to hide whatever the user is not authorized to view.
        /// </summary>
        /// <param name="content">The content being read and filtered for privacy.</param>
        void FilterUnauthorizedContent(T content);
    }
}
