// <copyright file="DebugModeOnlyFilter.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Filters
{
    using Fursvp.Domain.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Intercepts http and https calls and throws an exception if the current environment is not a Development environment.
    /// </summary>
    public class DebugModeOnlyFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugModeOnlyFilter"/> class.
        /// </summary>
        /// <param name="logger">The application event logger.</param>
        /// <param name="env">The web hosting environment.</param>
        public DebugModeOnlyFilter(ILogger<ApiExceptionFilter> logger, IWebHostEnvironment env)
        {
            this.Logger = logger;
            this.WebHostEnvironment = env;
        }

        private ILogger<ApiExceptionFilter> Logger { get; }

        private IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// Throws an exception if the environment is not Development.
        /// </summary>
        /// <param name="context">The ActionExecutingContext.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!this.WebHostEnvironment.IsDevelopment())
            {
                this.Logger.LogWarning("Attempt to access a debug-only controller.", context);
                throw new NotAuthorizedException<string>(string.Empty);
            }

            base.OnActionExecuting(context);
        }
    }
}
