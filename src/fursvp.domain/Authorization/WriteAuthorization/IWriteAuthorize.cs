// <copyright file="IWriteAuthorize.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.WriteAuthorization
{
    /// <summary>
    /// Checks for authorization by a given actor to perform a change to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type against which an attempted action must be authorized.</typeparam>
    public interface IWriteAuthorize<T>
    {
        /// <summary>
        /// Performs the authorization check for a state change and throws an exception if the check fails.
        /// </summary>
        /// <param name="oldState">The initial object state.</param>
        /// <param name="newState">The new object state.</param>
        void WriteAuthorize(T oldState, T newState);
    }
}
