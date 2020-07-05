// <copyright file="FormResponses.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a question and a list of responses to that question.
    /// </summary>
    public class FormResponses
    {
        /// <summary>
        /// Gets or sets the text prompt or question.
        /// </summary>
        public Guid PromptId { get; set; }

        /// <summary>
        /// Gets or sets the collection of text responses to the question.
        /// </summary>
        public ICollection<string> Responses { get; } = new Collection<string>();
    }
}
