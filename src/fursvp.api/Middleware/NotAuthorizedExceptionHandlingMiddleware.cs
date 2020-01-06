using fursvp.domain.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fursvp.api.Middleware
{
    public class NotAuthorizedExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public NotAuthorizedExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotAuthorizedException ex)
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync(ex.Message);
            }
        }
    }
}
