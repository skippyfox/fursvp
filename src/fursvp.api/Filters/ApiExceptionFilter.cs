using fursvp.domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fursvp.api.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<ApiExceptionFilter> _logger { get; }

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is NotAuthorizedException authEx)
            {
                OnException(context, StatusCodes.Status401Unauthorized, authEx.GetType().Name, authEx.Message, authEx.Type.Name);

                _logger?.LogInformation(authEx, authEx.Message);
            }
            else
            {
                var ex = context.Exception;
                OnException(context, StatusCodes.Status500InternalServerError, ex.GetType().Name, ex.Message, null);

                _logger?.LogError(ex, ex.Message);
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
