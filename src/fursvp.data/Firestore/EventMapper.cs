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

    /// <summary>
    /// Converts between the Event domain object and <see cref="Dictionary{String, Object}" />.
    /// </summary>
    public class EventMapper : IDictionaryMapper<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMapper"/> class.
        /// </summary>
        public EventMapper()
        {
        }

        /// <summary>
        /// Converts from Dictionary to Event.
        /// </summary>
        /// <param name="dictionary">The dictionary object that will be converted to Event.</param>
        /// <returns>The Event object on success, or null when dictionary is null.</returns>
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
                    RsvpedAt = m["RsvpedAt"] is Timestamp rsvpedAt ? rsvpedAt.ToDateTime() : DateTime.MinValue,
                    Responses = ((List<object>)m["Responses"]).Cast<Dictionary<string, object>>().Select(r => new FormResponses
                    {
                        PromptId = Guid.Parse((string)r["PromptId"]),
                        Responses = (List<string>)m["Responses"],
                    }).ToList(),
                }).ToList(),
                Form = ((List<object>)dictionary["Form"]).Cast<Dictionary<string, object>>().Select(r => new FormPrompt((string)r["Behavior"])
                    {
                        Id = Guid.Parse((string)r["Id"]),
                        Prompt = (string)r["Prompt"],
                        Options = (List<string>)r["Options"],
                        Required = r.TryGetValue("Required", out object required) ? (bool)required : false,
                        SortOrder = r.TryGetValue("SortOrder", out object sortOrder) ? Convert.ToInt32((long)sortOrder) : 0,
                    }).ToList(),
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

        /// <summary>
        /// Converts from Event to Dictionary.
        /// </summary>
        /// <param name="event">The Event to convert.</param>
        /// <returns>The Dictionary of variables.</returns>
        public Dictionary<string, object> ToDictionary(Event @event) => new Dictionary<string, object>
        {
            { nameof(@event.Id), @event.Id.ToString() },
            { nameof(@event.Version), @event.Version },
            { nameof(@event.StartsAt), @event.StartsAt },
            { nameof(@event.EndsAt), @event.EndsAt },
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
                    { nameof(m.RsvpedAt), m.RsvpedAt },
                    {
                        nameof(m.Responses), m.Responses.Select(r => new Dictionary<string, object>
                        {
                            { nameof(r.PromptId), r.PromptId },
                            { nameof(r.Responses), r.Responses.ToList() },
                        }).ToList()
                    },
                }).ToList()
            },
            {
                nameof(@event.Form), @event.Form.Select(f => new Dictionary<string, object>
                {
                    { nameof(f.Id), f.Id.ToString() },
                    { nameof(f.Behavior), f.Behavior },
                    { nameof(f.Prompt), f.Prompt },
                    { nameof(f.Options), f.Options.ToList() },
                    { nameof(f.SortOrder), f.SortOrder },
                    { nameof(f.Required), f.Required },
                }).ToList()
            },
            { nameof(@event.Name), @event.Name },
            { nameof(@event.OtherDetails), @event.OtherDetails },
            { nameof(@event.Location), @event.Location },
            { nameof(@event.RsvpOpen), @event.RsvpOpen },
            { nameof(@event.RsvpClosesAt), @event.RsvpClosesAt },
            { nameof(@event.IsPublished), @event.IsPublished },
        };
    }
}
