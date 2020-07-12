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

    /// <summary>
    /// An action filter that invokes read authorization on content that is subject to it, to hide private content.
    /// </summary>
    public class PrivateContentFilter : IActionFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateContentFilter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PrivateContentFilter(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Ensures that the user is authorized to read a single element and filters out any unauthorized content within the element.
        /// </summary>
        /// <typeparam name="T">The type of the content to be searched.</typeparam>
        /// <param name="content">The content being searched.</param>
        /// <param name="readAuthorizeObject">Executes read authorization methods against this type of object.</param>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext.</param>
        public static void Filter<T>(T content, IReadAuthorize<T> readAuthorizeObject, ActionExecutedContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (readAuthorizeObject == null)
            {
                throw new ArgumentNullException(nameof(readAuthorizeObject));
            }

            if (!readAuthorizeObject.CanRead(content))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            readAuthorizeObject.FilterUnauthorizedContent(content);
        }

        /// <summary>
        /// Filters out any unauthorized content within the collection of elements.
        /// </summary>
        /// <typeparam name="T">The type of content to be searched.</typeparam>
        /// <param name="objects">The content collection being searched.</param>
        /// <param name="readAuthorizeObject">Executes read authorization methods against this type of object.</param>
        /// <param name="contextResult">The context result object that contains the content collection value to be filtered.</param>
        public static void FilterMany<T>(IEnumerable<T> objects, IReadAuthorize<T> readAuthorizeObject, ObjectResult contextResult)
        {
            if (contextResult == null)
            {
                throw new ArgumentNullException(nameof(contextResult));
            }

            if (readAuthorizeObject == null)
            {
                throw new ArgumentNullException(nameof(readAuthorizeObject));
            }

            var filteredObjects = objects.Where(readAuthorizeObject.CanRead).ToList();

            foreach (var @object in filteredObjects)
            {
                readAuthorizeObject.FilterUnauthorizedContent(@object);
            }

            contextResult.Value = filteredObjects;
        }

        /// <summary>
        /// Searches the response for content that implements IReadAuthorize, and invokes IReadAuthorize methods to hide private content.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context?.Result is ObjectResult objectResult))
            {
                return;
            }

            if (objectResult.Value == null)
            {
                return;
            }

            // Try to filter a single Event being returned
            var objectType = objectResult.Value.GetType();
            if (objectType != null)
            {
                object readAuthorize = GetReadAuthorizeService(objectType);

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
                object readAuthorize = GetReadAuthorizeService(objectType);

                if (readAuthorize != null)
                {
                    var method = typeof(PrivateContentFilter).GetMethod(nameof(FilterMany));
                    var genericMethod = method.MakeGenericMethod(objectType);
                    genericMethod.Invoke(null, new object[] { objectResult.Value, readAuthorize, context.Result });

                    return;
                }
            }
        }

        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext.</param>
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

        private object GetReadAuthorizeService(Type objectType)
        {
            Type readAuthorizeType = typeof(IReadAuthorize<>).MakeGenericType(objectType);
            var readAuthorize = ServiceProvider.GetService(readAuthorizeType);
            return readAuthorize;
        }
    }
}
