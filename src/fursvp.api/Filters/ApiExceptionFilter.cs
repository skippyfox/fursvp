// <copyright file="ApiExceptionFilter.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Filters
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fursvp.Communication;
    using Fursvp.Domain.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
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
        /// <param name="emailer">The sender or suppressor of emails.</param>
        /// <param name="userAccessor">Accesses the current user info.</param>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IEmailer emailer, IUserAccessor userAccessor, IWebHostEnvironment webHostEnvironment)
        {
            this.Logger = logger;
            this.Emailer = emailer;
            this.UserAccessor = userAccessor;
            this.WebHostEnvironment = webHostEnvironment;
        }

        private ILogger<ApiExceptionFilter> Logger { get; }

        private IEmailer Emailer { get; }

        private IUserAccessor UserAccessor { get; }

        private IWebHostEnvironment WebHostEnvironment { get; }

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
            else if (context.Exception is ValidationException validationEx)
            {
                this.OnException(context, StatusCodes.Status400BadRequest, validationEx.GetType().Name, validationEx.Message);

                this.Logger?.LogInformation(validationEx, validationEx.Message);
            }
            else
            {
                var ex = context.Exception;
                string message = this.WebHostEnvironment.IsDevelopment()
                    ? ex.Message
                    : "An internal server error occurred. Sorry about that. The error has been logged.";

                this.OnException(context, StatusCodes.Status500InternalServerError, ex.GetType().Name, message);
                this.Logger?.LogError(ex, ex.Message);

                // TODO - put all of these values in config:
                try
                {
                    this.Emailer?.Send(new Email
                    {
                        From = new EmailAddress { Address = "noreply@fursvp.com", Name = "Fursvp.com" },
                        To = new EmailAddress { Address = "where.is.skippy@gmail.com" },
                        Subject = "Error on FURsvp.com",
                        PlainTextContent = @$"FURsvp error.

Time: {DateTime.Now}
Method: {context?.HttpContext?.Request?.Method}
QueryString: {context?.HttpContext?.Request?.QueryString}
Path: {context?.HttpContext?.Request?.Path}
Client IP: {context?.HttpContext?.Connection?.RemoteIpAddress}
User: {this.UserAccessor?.User?.EmailAddress}
Trace Identifier: {context?.HttpContext?.TraceIdentifier}

{ex.ToString()}

Inner Exception: {(ex.InnerException == null ? "null" : ex.InnerException.ToString())}",
                        HtmlContent = @$"<p>FURsvp error.</p>
<p>Time: {DateTime.Now}
<br />Method: {context?.HttpContext?.Request?.Method}
<br />QueryString: {context?.HttpContext?.Request?.QueryString}
<br />Path: {context?.HttpContext?.Request?.Path}
<br />Client IP: {context?.HttpContext?.Connection?.RemoteIpAddress}
<br />User: {this.UserAccessor?.User?.EmailAddress}
<br />Trace Identifier: {context?.HttpContext?.TraceIdentifier}</p>
<p>{ex.ToString().Replace("\n", "<br />")}</p>
<p>Inner Exception: {(ex.InnerException == null ? "null" : ex.InnerException.ToString().Replace("\n", "<br />"))}</p>",
                    });
                }
                catch
                {
                    // TODO ?
                }
            }

            base.OnException(context);
        }

        private void OnException(ExceptionContext context, int statusCode, string exception, string errorMessage, string entity = null)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(new { exception, errorMessage, entity });
        }
    }
}
