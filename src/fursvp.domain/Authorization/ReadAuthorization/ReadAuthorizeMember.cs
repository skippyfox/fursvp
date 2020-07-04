using System;
using System.Collections.Generic;
using System.Text;

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    public class ReadAuthorizeMember : IReadAuthorize<Member>
    {
        private readonly IUserAccessor userAccessor;

        public ReadAuthorizeMember(IUserAccessor userAccessor)
        {
            this.userAccessor = userAccessor;
        }

        public bool CanRead(Member member) => true;

        public void FilterUnauthorizedContent(Member member)
        {
            var user = this.userAccessor.User;

            if (member.EmailAddress != user?.EmailAddress)
            {
                member.EmailAddress = null;
            }
        }
    }
}
