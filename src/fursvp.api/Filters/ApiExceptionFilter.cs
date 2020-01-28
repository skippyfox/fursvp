// <copyright file="ApiExceptionFilter.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Filters
{
    using Fursvp.Domain.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Intercepts http and https calls when an exception is thrown.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger">The application event logger.</param>
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            this.Logger = logger;
        }

        private ILogger<ApiExceptionFilter> Logger { get; }

        /// <summary>
        /// Handle an Exception caught by MVC. Called by MVC when an otherwise uncaught exception is thrown.
        /// </summary>
        /// <param name="context">The Exception context.</param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is NotAuthorizedException authEx)
            {
                this.OnException(context, StatusCodes.Status401Unauthorized, authEx.GetType().Name, authEx.Message, authEx.Type.Name);

                this.Logger?.LogInformation(authEx, authEx.Message);
            }
            else
            {
                var ex = context.Exception;
                this.OnException(context, StatusCodes.Status500InternalServerError, ex.GetType().Name, ex.Message, null);

                this.Logger?.LogError(ex, ex.Message);
            }

            base.OnException(context);
        }

        private void OnException(ExceptionContext context, int statusCode, string exception, string errorMessage, string entity)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(new { exception, errorMessage, entity });
        }
    }
}
