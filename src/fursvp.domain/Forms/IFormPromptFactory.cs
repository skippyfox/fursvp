// <copyright file="IFormPromptFactory.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides instantiation logic for <see cref="FormPrompt"/> implementations based on a discriminator value.
    /// </summary>
    public interface IFormPromptFactory
    {
        /// <summary>
        /// Initializes a new instance of the abstract <see cref="FormPrompt"/> using an implementation determined by a discriminator value.
        /// </summary>
        /// <param name="discriminator">The discriminator value.</param>
        /// <param name="prompt">The text prompt or question.</param>
        /// <param name="options">The collection of text choices.</param>
        /// <returns>The newly instantiated <see cref="FormPrompt"/> implementation.</returns>
        FormPrompt GetFormPrompt(string discriminator, string prompt, ICollection<string> options);
    }
}
