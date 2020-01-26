// <copyright file="NewMemberRequest.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class NewMemberRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
