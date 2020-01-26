// <copyright file="IAuthorize.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization
{
    public interface IAuthorize<T>
    {
        void Authorize(string actor, T oldState, T newState);
    }
}
