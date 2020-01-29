// <copyright file="FormPrompt.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a question and a list of possible answers.
    /// </summary>
    public abstract class FormPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormPrompt"/> class.
        /// </summary>
        /// <param name="behavior">The descriminator that indicates which implementation of FormPrompt to persist.</param>
        public FormPrompt(string behavior)
        {
            this.Behavior = behavior;
        }

        /// <summary>
        /// Gets the discriminator for implementations of FormPrompt.
        /// </summary>
        public string Behavior { get; }

        /// <summary>
        /// Gets or sets the text prompt or question.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the collection of text choices.
        /// </summary>
        public ICollection<string> Options { get; set; } = new Collection<string>();
    }
}
