// <copyright file="EventMapper.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.Firestore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fursvp.Domain;
    using Fursvp.Domain.Forms;
    using Google.Cloud.Firestore;

    public class EventMapper : IDictionaryMapper<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMapper"/> class.
        /// </summary>
        /// <param name="formPromptFactory">An instance of <see cref="IFormPromptFactory"/>.</param>
        public EventMapper(IFormPromptFactory formPromptFactory)
        {
            this.FormPromptFactory = formPromptFactory;
        }

        private IFormPromptFactory FormPromptFactory { get; }

        public Event FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            var result = new Event
            {
                Id = Guid.Parse((string)dictionary["Id"]),
                Version = Convert.ToInt32((long)dictionary["Version"]),
                StartsAt = ((Timestamp)dictionary["StartsAt"]).ToDateTime(),
                EndsAt = ((Timestamp)dictionary["EndsAt"]).ToDateTime(),
                TimeZoneId = (string)dictionary["TimeZoneId"],
                Members = ((List<object>)dictionary["Members"]).Cast<Dictionary<string, object>>().Select(m => new Member
                {
                    Id = Guid.Parse((string)m["Id"]),
                    EmailAddress = (string)m["EmailAddress"],
                    Name = (string)m["Name"],
                    IsAttending = (bool)m["IsAttending"],
                    IsOrganizer = (bool)m["IsOrganizer"],
                    IsAuthor = (bool)m["IsAuthor"],
                    Responses = ((List<object>)m["Responses"]).Cast<Dictionary<string, object>>().Select(r => new FormResponses
                    {
                        Prompt = (string)r["Prompt"],
                        Responses = (List<string>)m["Responses"],
                    }).ToList(),
                }).ToList(),
                Form = ((List<object>)dictionary["Form"]).Cast<Dictionary<string, object>>().Select(r => this.FormPromptFactory.GetFormPrompt(
                    discriminator: (string)r["Behavior"],
                    prompt: (string)r["Prompt"],
                    options: (List<string>)r["Options"])).ToList(),
                Name = (string)dictionary["Name"],
                OtherDetails = (string)dictionary["OtherDetails"],
                Location = (string)dictionary["Location"],
                RsvpOpen = (bool)dictionary["RsvpOpen"],
                IsPublished = (bool)dictionary["IsPublished"],
            };

            if (dictionary["RsvpClosesAt"] is Timestamp rsvpClosesAt)
            {
                result.RsvpClosesAt = rsvpClosesAt.ToDateTime();
            }

            return result;
        }

        public Dictionary<string, object> ToDictionary(Event @event) => new Dictionary<string, object>
        {
            { nameof(@event.Id), @event.Id.ToString() },
            { nameof(@event.Version), @event.Version },
            { nameof(@event.StartsAt), @event.StartsAt.ToUniversalTime() },
            { nameof(@event.EndsAt), @event.EndsAt.ToUniversalTime() },
            { nameof(@event.TimeZoneId), @event.TimeZoneId },
            {
                nameof(@event.Members), @event.Members.Select(m => new Dictionary<string, object>
                {
                    { nameof(m.Id), m.Id.ToString() },
                    { nameof(m.EmailAddress), m.EmailAddress },
                    { nameof(m.Name), m.Name },
                    { nameof(m.IsAttending), m.IsAttending },
                    { nameof(m.IsOrganizer), m.IsOrganizer },
                    { nameof(m.IsAuthor), m.IsAuthor },
                    {
                        nameof(m.Responses), m.Responses.Select(r => new Dictionary<string, object>
                        {
                            { nameof(r.Prompt), r.Prompt },
                            { nameof(r.Responses), r.Responses.ToList() },
                        }).ToList()
                    },
                }).ToList()
            },
            {
                nameof(@event.Form), @event.Form.Select(f => new Dictionary<string, object>
                {
                    { nameof(f.Behavior), f.Behavior },
                    { nameof(f.Prompt), f.Prompt },
                    { nameof(f.Options), f.Options.ToList() },
                }).ToList()
            },
            { nameof(@event.Name), @event.Name },
            { nameof(@event.OtherDetails), @event.OtherDetails },
            { nameof(@event.Location), @event.Location },
            { nameof(@event.RsvpOpen), @event.RsvpOpen },
            { nameof(@event.RsvpClosesAt), @event.RsvpClosesAt?.ToUniversalTime() },
            { nameof(@event.IsPublished), @event.IsPublished },
        };
    }
}
