using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fursvp.api.Filters;
using fursvp.data;
using fursvp.domain;
using fursvp.domain.Authorization;
using fursvp.domain.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fursvp.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IEventService, EventService>();
            ConfigureRepositoryServices(services);
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

        private void ConfigureRepositoryServices(IServiceCollection services)
        {
            services.AddSingleton<IRepository<Event>>(s => {
                var baseEventRepository = new FakeEventRepository();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
    }
}
