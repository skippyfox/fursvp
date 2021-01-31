// <copyright file="Startup.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api
{
    using System.Text;
    using AutoMapper;
    using Fursvp.Api.Filters;
    using Fursvp.Api.Responses;
    using Fursvp.Communication;
    using Fursvp.Domain.Authorization;
    using Fursvp.Domain.Authorization.ReadAuthorization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

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
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The container in which components are registered.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddDomainServices();
            services.AddFursvpDataWithFirestore();

            if (Environment.IsDevelopment() || string.IsNullOrEmpty(Configuration["SendGrid:ApiKey"]))
            {
                services.AddSingleton<IEmailer, SuppressAndLogEmailer>();
            }
            else
            {
                services.AddSingleton<IEmailer, SendGridEmailer>();
                services.Configure<SendGridOptions>(Configuration.GetSection(SendGridOptions.SendGrid));
            }

            services.AddLogging(lc =>
            {
                lc.ClearProviders();
                lc.AddConsole();
            });

            services.AddSingleton<PrivateContentFilter>();
            services.AddSingleton<DebugModeOnlyFilter>();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilter));
                options.Filters.AddService<PrivateContentFilter>();
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x => x.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(x.GetService<IActionContextAccessor>().ActionContext));
            services.AddHttpContextAccessor(); // For authorization / access to user info.
            services.AddSingleton<IUserAccessor, ClaimsPrincipalUserAccessor>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["AuthorizationIssuerSigningKey"]);
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSingleton<IReadAuthorize<EventResponse>, ReadAuthorizeEvent<EventResponse, MemberResponse>>();
            services.AddSingleton<IReadAuthorize<MemberResponse>, ReadAuthorizeMember<MemberResponse>>();
            ConfigureAutoMapper(services);
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<Fursvp.Data.MappingProfile>();
                cfg.AddProfile<Responses.ResponseMappingProfile>();
            });
            services.AddSingleton(mappingConfig.CreateMapper());
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
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
