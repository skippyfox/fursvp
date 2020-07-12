// <copyright file="ReadAuthorizeMember.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

using System;

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    /// <summary>
    /// Authorizes and filters access to objects of type <see cref="Member" />.
    /// </summary>
    public class ReadAuthorizeMember<T> : IReadAuthorize<T>
        where T : IReadAuthorizableMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadAuthorizeMember"/> class.
        /// </summary>
        /// <param name="userAccessor">An instance of IUserAccessor to identify user access.</param>
        public ReadAuthorizeMember(IUserAccessor userAccessor)
        {
            UserAccessor = userAccessor;
        }

        private IUserAccessor UserAccessor { get; }

        /// <summary>
        /// Indicates whether the current user is allowed to view any information related to this member.
        /// </summary>
        /// <param name="member">The member information being viewed.</param>
        /// <returns>A value indicating whether the user is allowed to view any information related to this member.</returns>
        public bool CanRead(T member) => true;

        /// <summary>
        /// Finds any unauthorized content within the Member object and redacts it if the user is not authorized to view it.
        /// </summary>
        /// <param name="member">The member information being viewed.</param>
        public void FilterUnauthorizedContent(T member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            var user = UserAccessor.User;

            if (member.EmailAddress != user?.EmailAddress)
            {
                member.EmailAddress = null;
            }
        }
    }
}
