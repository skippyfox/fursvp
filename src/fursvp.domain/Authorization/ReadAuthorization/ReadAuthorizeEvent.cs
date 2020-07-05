// <copyright file="ReadAuthorizeEvent.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    using System.Linq;

    public class ReadAuthorizeEvent : IReadAuthorize<Event>
    {
        public ReadAuthorizeEvent(IUserAccessor userAccessor, IReadAuthorize<Member> readAuthorizeMember)
        {
            this.UserAccessor = userAccessor;
            this.ReadAuthorizeMember = readAuthorizeMember;
        }

        private IUserAccessor UserAccessor { get; }

        private IReadAuthorize<Member> ReadAuthorizeMember { get; }

        public bool CanRead(Event @event)
        {
            if (@event == null || @event.IsPublished == true)
            {
                return true;
            }

            const bool organizersCanViewUnpublishedEvent = false;

            var actingMember = @event.Members.FirstOrDefault(m => m.EmailAddress == this.UserAccessor.User?.EmailAddress);

            if (actingMember?.IsAuthor == true)
            {
                return true;
            }

            if (actingMember?.IsOrganizer == true)
            {
                return organizersCanViewUnpublishedEvent;
            }

            return false;
        }

        public void FilterUnauthorizedContent(Event @event)
        {
            if (@event?.Members == null)
            {
                return;
            }

            @event.Members = @event.Members.Where(this.ReadAuthorizeMember.CanRead).ToList();

            foreach (var member in @event.Members)
            {
                this.ReadAuthorizeMember.FilterUnauthorizedContent(member);
            }
        }
    }
}
