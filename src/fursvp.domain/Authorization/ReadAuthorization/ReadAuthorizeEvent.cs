using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    public class ReadAuthorizeEvent : IReadAuthorize<Event>
    {
        private readonly IUserAccessor userAccessor;
        private readonly IReadAuthorize<Member> readAuthorizeMember;

        public ReadAuthorizeEvent(IUserAccessor userAccessor, IReadAuthorize<Member> readAuthorizeMember)
        {
            this.userAccessor = userAccessor;
            this.readAuthorizeMember = readAuthorizeMember;
        }

        public bool CanRead(Event @event)
        {
            if (@event == null || @event.IsPublished == true)
            {
                return true;
            }

            const bool organizersCanViewUnpublishedEvent = false; // TODO - Do we want this?

            var actingMember = @event.Members.FirstOrDefault(m => m.EmailAddress == this.userAccessor.User?.EmailAddress);

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

            @event.Members = @event.Members.Where(this.readAuthorizeMember.CanRead).ToList();

            foreach (var member in @event.Members)
            {
                this.readAuthorizeMember.FilterUnauthorizedContent(member);
            }
        }
    }
}
