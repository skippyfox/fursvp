// <copyright file="FormPromptFactory.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides instantiation logic for <see cref="FormPrompt"/> implementations based on a discriminator value.
    /// </summary>
    public class FormPromptFactory : IFormPromptFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormPromptFactory"/> class.
        /// </summary>
        public FormPromptFactory()
        {
            this.FormPrompts = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FormPrompt)))
                .ToDictionary(t => t.Name, t => t);
        }

        private Dictionary<string, Type> FormPrompts { get; }

        /// <summary>
        /// Initializes a new instance of the abstract <see cref="FormPrompt"/> using an implementation determined by a discriminator value.
        /// </summary>
        /// <param name="discriminator">The discriminator value.</param>
        /// <param name="prompt">The text prompt or question.</param>
        /// <param name="options">The collection of text choices.</param>
        /// <returns>The newly instantiated <see cref="FormPrompt"/> implementation.</returns>
        public FormPrompt GetFormPrompt(string discriminator, string prompt, ICollection<string> options)
        {
            var instance = (FormPrompt)Activator.CreateInstance(this.FormPrompts[discriminator]);
            instance.Prompt = prompt;
            instance.Options = options;
            return instance;
        }
    }
}
