// <copyright file="DependencyInjection.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization.ReadAuthorization;
    using Fursvp.Domain.Authorization.WriteAuthorization;
    using Fursvp.Domain.Validation;

    /// <summary>
    /// Provides static DependencyInjection extension methods for installation with .net core service collection.
    /// </summary>
    public static class DependencyInjectionInstaller
    {
        /// <summary>
        /// Registers services for use with the Fursvp Domain.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IProvideDateTime, UtcDateTimeProvider>();

            services.AddSingleton<IValidateTimeZone, ValidateTimeZone>();
            services.AddSingleton<IValidateEmail, ValidateEmail>();

            services.AddSingleton<IEventService, EventService>();

            services.AddSingleton<IReadAuthorize<Event>, ReadAuthorizeEvent>();
            services.AddSingleton<IReadAuthorize<Member>, ReadAuthorizeMember>();

            services.AddSingleton<IWriteAuthorize<Event>, WriteAuthorizeEvent>();
            services.AddSingleton<IWriteAuthorizeMember, WriteAuthorizeMember>();

            services.AddSingleton<IValidate<Event>, ValidateEvent>();
            services.AddSingleton<IValidateMember, ValidateMember>();
        }
    }
}
