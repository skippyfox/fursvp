// <copyright file="PrivateContentFilter.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fursvp.Domain.Authorization.ReadAuthorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class PrivateContentFilter : IActionFilter
    {
        private readonly IServiceProvider serviceProvider;

        public PrivateContentFilter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is ObjectResult objectResult))
            {
                return;
            }

            // Try to filter a single Event being returned
            var objectType = objectResult.Value.GetType();
            if (objectType != null)
            {
                object readAuthorize = this.GetReadAuthorizeService(objectType);

                if (readAuthorize != null)
                {
                    var method = typeof(PrivateContentFilter).GetMethod(nameof(Filter));
                    var genericMethod = method.MakeGenericMethod(objectType);
                    genericMethod.Invoke(null, new object[] { objectResult.Value, readAuthorize, context });

                    return;
                }
            }

            // Try to filter multiple objects being returned
            objectType = IEnumerableGenericArgument(objectType);
            if (objectType != null)
            {
                object readAuthorize = this.GetReadAuthorizeService(objectType);

                if (readAuthorize != null)
                {
                    var method = typeof(PrivateContentFilter).GetMethod(nameof(FilterMany));
                    var genericMethod = method.MakeGenericMethod(objectType);
                    genericMethod.Invoke(null, new object[] { objectResult.Value, readAuthorize, context.Result });

                    return;
                }
            }
        }

        private object GetReadAuthorizeService(Type objectType)
        {
            Type readAuthorizeType = typeof(IReadAuthorize<>).MakeGenericType(objectType);
            var readAuthorize = this.serviceProvider.GetService(readAuthorizeType);
            return readAuthorize;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        private static Type IEnumerableGenericArgument(Type candidate)
        {
            return candidate.GetInterfaces()
                .Append(candidate)
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments())
                .FirstOrDefault(args => args.Length == 1)
                ?.Single();
        }

        public static void Filter<T>(T @object, IReadAuthorize<T> readAuthorizeObject, ActionExecutedContext context)
        {
            if (!readAuthorizeObject.CanRead(@object))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            readAuthorizeObject.FilterUnauthorizedContent(@object);
        }

        public static void FilterMany<T>(IEnumerable<T> objects, IReadAuthorize<T> readAuthorizeObject, ObjectResult contextResult)
        {
            var filteredObjects = objects.Where(readAuthorizeObject.CanRead).ToList();

            foreach (var @object in filteredObjects)
            {
                readAuthorizeObject.FilterUnauthorizedContent(@object);
            }

            contextResult.Value = filteredObjects;
        }
    }
}
