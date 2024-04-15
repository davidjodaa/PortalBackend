using AspNetCoreRateLimit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using PortalBackend.Domain.Settings;
using PortalBackend.Infraucture.Extension;
using PortalBackend.Persistence;
using PortalBackend.Service;
using Serilog;
using System;
using System.IO;
using FluentValidation.AspNetCore;
using PortalBackend.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Hangfire;

namespace PortalBackend
{
    public class Startup
    {
        private readonly IConfigurationRoot configRoot;
        private AppSettings AppSettings { get; set; }

        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;

            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            configRoot = builder.Build();

            AppSettings = new AppSettings();
            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext(Configuration, configRoot);

            services.AddValidation();

            services.AddIdentityService(Configuration);

            services.AddTransientServices();

            services.AddSingletonServices();

            services.AddAutoMapper();

            services.AddScopedServices();

            services.AddHostedServices();

            services.AddSwaggerOpenAPI();

            services.AddServiceLayer(Configuration);

            //services.AddHangfire(Configuration, configRoot);

            //Register MVC/Web API, NewtonsoftJson and add FluentValidation Support
            services.AddFluentControllers();

            services.AddVersion();

            //services.AddHTTPPolicies(Configuration);

            services.AddRequestRateLimiter(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory log)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureCors(Configuration);

            app.ConfigureCustomExceptionMiddleware();

            log.AddSerilog();

            app.UseRouting();

            app.UseIpRateLimiting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.ConfigureSwagger();
            //app.ConfigureHangfire();
            //app.UseHealthChecks("/healthz", new HealthCheckOptions
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            //    ResultStatusCodes =
            //    {
            //        [HealthStatus.Healthy] = StatusCodes.Status200OK,
            //        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
            //        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            //    },
            //}).UseHealthChecksUI(setup =>
            //  {
            //      setup.ApiPath = "/healthcheck";
            //      setup.UIPath = "/healthcheck-ui";
            //      //setup.AddCustomStylesheet("Customization/custom.css");
            //  });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
