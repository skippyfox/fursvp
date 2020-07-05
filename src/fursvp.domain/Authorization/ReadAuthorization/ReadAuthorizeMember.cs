// <copyright file="ReadAuthorizeMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    public class ReadAuthorizeMember : IReadAuthorize<Member>
    {
        public ReadAuthorizeMember(IUserAccessor userAccessor)
        {
            this.UserAccessor = userAccessor;
        }

        private IUserAccessor UserAccessor { get; }

        public bool CanRead(Member member) => true;

        public void FilterUnauthorizedContent(Member member)
        {
            var user = this.UserAccessor.User;

            if (member.EmailAddress != user?.EmailAddress)
            {
                member.EmailAddress = null;
            }
        }
    }
}
