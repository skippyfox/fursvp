// <copyright file="MultipleChoicePrompt.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Forms
{
    /// <summary>
    /// Represents a question and a list of multiple choice answers where any number of answers may be chosen.
    /// </summary>
    public class MultipleChoicePrompt : FormPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleChoicePrompt"/> class.
        /// </summary>
        public MultipleChoicePrompt()
            : base(nameof(MultipleChoicePrompt))
        {
        }
    }
}
