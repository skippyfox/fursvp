// <copyright file="IDictionaryMapper.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.Firestore
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides conversion between <see cref="Dictionary{String, Object}"/> and type T for useu with Firestore.
    /// </summary>
    /// <typeparam name="T">The Entity to be converted.</typeparam>
    public interface IDictionaryMapper<T>
    {
        /// <summary>
        /// Converts an Entity of type T to <see cref="Dictionary{String, Object}"/>.
        /// </summary>
        /// <param name="object">An instance of the entity to convert to Dictionary.</param>
        /// <returns>The dictionary converted from the entity.</returns>
        Dictionary<string, object> ToDictionary(T @object);

        /// <summary>
        /// Converts an Entity of <see cref="Dictionary{String, Object}"/> to type T.
        /// </summary>
        /// <param name="dictionary">An instance of the dictionary to convert between two types.</param>
        /// <returns>The dictionary converted from the entity.</returns>
        T FromDictionary(Dictionary<string, object> dictionary);
    }
}
