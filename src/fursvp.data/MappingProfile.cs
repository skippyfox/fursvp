// <copyright file="MappingProfile.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using AutoMapper;
    using Fursvp.Domain;
    using Fursvp.Domain.Forms;

    /// <summary>
    /// Mapping Profile for use with AutoMapper, including deep copies of objects.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            CreateIdentityMapsForDeepCopying();
        }

        private void CreateIdentityMapsForDeepCopying()
        {
            CreateMap<Event, Event>();
            CreateMap<Member, Member>();
            CreateMap<FormPrompt, FormPrompt>();
            CreateMap<FormResponses, FormResponses>();
        }
    }
}