// <copyright file="IFormPromptFactory.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System.Collections.Generic;

    public interface IFormPromptFactory
    {
        FormPrompt GetFormPrompt(string discriminator, string prompt, ICollection<string> options);
    }
}
