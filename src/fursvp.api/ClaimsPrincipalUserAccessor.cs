// <copyright file="ClaimsPrincipalUserAccessor.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using Fursvp.Domain.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Provides information about a user via claims based authentication.
    /// </summary>
    public class ClaimsPrincipalUserAccessor : IUserAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsPrincipalUserAccessor"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HttpContextAccessor.</param>
        public ClaimsPrincipalUserAccessor(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            MemoryCache = memoryCache;
        }

        /// <summary>
        /// Gets the authenticated user, or null if the user is not authenticated.
        /// </summary>
        public User User
        {
            get
            {
                var claimsPrincipal = HttpContextAccessor.HttpContext.User;
                if (claimsPrincipal?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var sessionId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;
                if (sessionId == null || !MemoryCache.TryGetValue("SessionId:" + sessionId, out _))
                {
                    // User logged out or session has otherwise expired
                    return null;
                }

                return new User
                {
                    EmailAddress = claimsPrincipal.Identity.Name,
                    SessionId = sessionId,
                };
            }
        }

        private IHttpContextAccessor HttpContextAccessor { get; }
        private IMemoryCache MemoryCache { get; }
    }
}
