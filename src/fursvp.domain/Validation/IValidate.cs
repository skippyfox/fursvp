// <copyright file="IValidate.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Validation
{
    /// <summary>
    /// Compares and validates the transition between two states (instances of type T). For use with Domain validation, not endpoint request validation.
    /// </summary>
    /// <typeparam name="T">The type of which two instances are to be compared.</typeparam>
    public interface IValidate<T>
    {
        /// <summary>
        /// Compares two instances of T and throws an exception if the transition from oldState to newState is not valid.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        void ValidateState(T oldState, T newState);
    }
}
