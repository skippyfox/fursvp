// <copyright file="ClaimsPrincipalUserAccessor.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api
{
    using System;
    using System.Security.Claims;
    using Fursvp.Domain.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Provides information about a user via claims based authentication.
    /// </summary>
    public class ClaimsPrincipalUserAccessor : IUserAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsPrincipalUserAccessor"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HttpContextAccessor.</param>
        public ClaimsPrincipalUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

                return new User
                {
                    EmailAddress = claimsPrincipal.Identity.Name,
                };
            }
        }

        private IHttpContextAccessor HttpContextAccessor { get; }
    }
}
