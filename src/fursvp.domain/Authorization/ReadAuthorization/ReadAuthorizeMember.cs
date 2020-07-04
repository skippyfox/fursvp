// <copyright file="ReadAuthorizeMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

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
