// <copyright file="FormResponses.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class FormResponses
    {
        public string Prompt { get; set; }

        public ICollection<string> Responses { get; set; } = new Collection<string>();
    }
}
