// <copyright file="FormPrompt.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a question and a list of possible answers.
    /// </summary>
    public class FormPrompt
    {
        /// <summary>
        /// Gets or sets the globally unique identifier for the prompt.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the discriminator for implementations of FormPrompt.
        /// </summary>
        public string Behavior { get; set; }

        /// <summary>
        /// Gets or sets the text prompt or question.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a response is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the collection of text choices.
        /// </summary>
        public ICollection<string> Options { get; set;  } = new Collection<string>();

        /// <summary>
        /// Gets or sets the sort order of the prompt.
        /// </summary>
        public int SortOrder { get; set; }
    }
}
