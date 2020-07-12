// <copyright file="ReadAuthorizeEvent.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Domain.Authorization.ReadAuthorization
{
    using System.Linq;

    /// <summary>
    /// Authorizes and filters access to objects of type <see cref="Member" />.
    /// </summary>
    public class ReadAuthorizeEvent<TEvent, TMember> : IReadAuthorize<TEvent>
        where TEvent : IReadAuthorizableEvent<TMember>
        where TMember : IReadAuthorizableMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadAuthorizeEvent"/> class.
        /// </summary>
        /// <param name="userAccessor">An instance of IUserAccessor to identify user access.</param>
        /// <param name="readAuthorizeMember">An instance of IReadAuthorize to perform deeper authorization against members being viewed within this event.</param>
        public ReadAuthorizeEvent(IUserAccessor userAccessor, IReadAuthorize<TMember> readAuthorizeMember)
        {
            UserAccessor = userAccessor;
            ReadAuthorizeMember = readAuthorizeMember;
        }

        private IUserAccessor UserAccessor { get; }

        private IReadAuthorize<TMember> ReadAuthorizeMember { get; }

        /// <summary>
        /// Indicates whether the current user is allowed to view any information related to this event.
        /// </summary>
        /// <param name="event">The event information being viewed.</param>
        /// <returns>A value indicating whether the user is allowed to view any information related to this event.</returns>
        public bool CanRead(TEvent @event)
        {
            if (@event == null || @event.IsPublished == true)
            {
                return true;
            }

            const bool organizersCanViewUnpublishedEvent = false;

            var actingMember = @event.Members.FirstOrDefault(m => m.EmailAddress == UserAccessor.User?.EmailAddress);

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

        /// <summary>
        /// Finds any unauthorized content within the Event object and redacts it if the user is not authorized to view it.
        /// </summary>
        /// <param name="event">The event information being viewed.</param>
        public void FilterUnauthorizedContent(TEvent @event)
        {
            if (@event?.Members == null)
            {
                return;
            }

            foreach (var member in @event.Members.Where(m => !ReadAuthorizeMember.CanRead(m)).ToList()) 
            {
                @event.Members.Remove(member);
            }

            foreach (var member in @event.Members)
            {
                ReadAuthorizeMember.FilterUnauthorizedContent(member);
            }
        }
    }
}
