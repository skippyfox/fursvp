// <copyright file="Startup.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api
{
    using Fursvp.Api.Filters;
    using Fursvp.Data;
    using Fursvp.Data.Firestore;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization;
    using Fursvp.Domain.Forms;
    using Fursvp.Domain.Validation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Fursvp .NET Core Web Api initialization routines.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
        /// <param name="environment">The <see cref="IWebHostEnvironment"/> instance containing environment specific settings.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The container in which components are registered.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IEventService, EventService>();
            this.ConfigureRepositoryServices(services);
            services.AddSingleton<IValidateEmail, ValidateEmail>();
            services.AddSingleton<IProvideDateTime, UtcDateTimeProvider>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x => x.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(x.GetService<IActionContextAccessor>().ActionContext));
            services.AddLogging(lc =>
            {
                lc.ClearProviders();
                lc.AddConsole();
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilter));
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance to configure.</param>
        /// <param name="env">The <see cref="IWebHostEnvironment"/> instance containing environment specific settings.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureRepositoryServices(IServiceCollection services)
        {
            services.AddSingleton<IRepository<Event>>(s =>
            {
                //// var baseEventRepository = new InMemoryEventRepository(new FormPromptFactory());
                var baseEventRepository = new FirestoreRepository<Event>(new EventMapper(new FormPromptFactory()));
                var dateTimeProvider = s.GetRequiredService<IProvideDateTime>();
                var validateEmail = s.GetRequiredService<IValidateEmail>();
                var validateMember = new ValidateMember(validateEmail);
                var validateEvent = new ValidateEvent(dateTimeProvider, validateMember);

                var eventService = s.GetRequiredService<IEventService>();
                var authorizeEvent = new AuthorizeEvent(
                    new AuthorizeMemberAsAuthor(),
                    new AuthorizeMemberAsOrganizer(),
                    new AuthorizeMemberAsAttendee(),
                    new AuthorizeFrozenMemberAsAttendee(),
                    eventService);

                var authEventRepository = new RepositoryWithAuthorization<Event>(baseEventRepository, authorizeEvent);

                return new RepositoryWithValidation<Event>(authEventRepository, validateEvent);
            });
        }
    }
}
