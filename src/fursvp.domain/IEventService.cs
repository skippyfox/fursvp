// <copyright file="IEventService.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain
{
    public interface IEventService
    {
        void AddMember(Event @event, Member member);

        Event CreateNewEvent(string emailAddress, string name);

        bool RsvpOpen(Event @event);
    }
}