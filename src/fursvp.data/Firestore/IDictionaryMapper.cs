// <copyright file="IDictionaryMapper.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.Firestore
{
    using System.Collections.Generic;

    public interface IDictionaryMapper<T>
    {
        Dictionary<string, object> ToDictionary(T @object);

        T FromDictionary(Dictionary<string, object> dictionary);
    }
}
