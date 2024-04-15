using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using PortalBackend.Domain.Settings;
using PortalBackend.Infrastructure.Configs;
using FluentValidation.AspNetCore;
using PortalBackend.Service.Contract;
using PortalBackend.Service.Implementation;
using System;
using System.Collections.Generic;
using FluentValidation;
using PortalBackend.Service.DTO.Request;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Hangfire;
using PortalBackend.Persistence;
using System.Reflection;
using Hangfire.MemoryStorage;
using PortalBackend.Domain.Auth;
using PortalBackend.Domain.QueryParameters;
using System.Linq;
using PortalBackend.Service.Exceptions;
using PortalBackend.Infrastructure.Interceptor;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PortalBackend.Service.BackgroundServices;

namespace PortalBackend.Infrastructure.Extension
{
    public static class ConfigureServiceContainer
    {
        public static void AddDbContext(this IServiceCollection serviceCollection,
             IConfiguration configuration, IConfigurationRoot configRoot)
        {
            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseOracle(configuration
                    .GetConnectionString("DBConnectionString") ?? configRoot["ConnectionStrings:DBConnectionString"]
                , b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.UseOracleSQLCompatibility("11");
                    b.MigrationsHistoryTable("__AMMigrationsHistory", "");
                });
                options.AddInterceptors(serviceCollection.BuildServiceProvider().GetRequiredService<DBLongQueryLogger>());
            });
        }

        public static void AddHangfire(this IServiceCollection serviceCollection)
        {
            var hangfireDbContext = serviceCollection.BuildServiceProvider().GetService<IApplicationDbContext>();

            serviceCollection.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());

            serviceCollection.AddHangfireServer(options =>
            {
                options.ServerName = String.Format("{0}.{1}", Environment.MachineName, Guid.NewGuid().ToString());
                options.WorkerCount = 1;
                options.Queues = new[] {
                      "default",
                    };
            });
        }
        
        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDateTimeService, DateTimeService>();
            serviceCollection.AddTransient<IClientFactory, ClientFactory>();
        }

        public static void AddSingletonServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IQueueManager, QueueManager>();
        }

        public static void AddHostedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<EmailBackgroundService>();
        }

        public static void AddAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(MappingProfileConfiguration));
        }

        public static void AddScopedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            serviceCollection.AddScoped<DBLongQueryLogger>();
            serviceCollection.AddScoped<IAccountService, AccountService>();
            serviceCollection.AddScoped<IAppSessionService, AppSessionService>();
            serviceCollection.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
            serviceCollection.AddScoped<IAPIImplementation, APIImplementations>();
            serviceCollection.AddScoped<IPinService, PinService>();
            serviceCollection.AddScoped<IReportService, ReportService>();
            serviceCollection.AddScoped<IAuditService, AuditService>();
            serviceCollection.AddScoped<IUtilityService, UtilityService>();
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<IEnquiryService, EnquiryService>();
            serviceCollection.AddScoped<IUnlockService, UnlockService>();
            serviceCollection.AddScoped<IProfileUpdateService, ProfileUpdateService>();
            serviceCollection.AddScoped<ITemplateService, TemplateService>();
        }

        public static void AddValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();
            serviceCollection.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            serviceCollection.AddScoped<IValidator<AddPinRequest>, AddPinRequestValidator>();
            serviceCollection.AddScoped<IValidator<ReportRequest>, ReportRequestValidator>();
            serviceCollection.AddScoped<IValidator<AcctOpenRptQueryParameters>, AcctOpenRptQueryParametersValidator>();

            //Disable Automatic Model State Validation built-in to ASP.NET Core
            serviceCollection.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = false; });
        }

        public static void AddSwaggerOpenAPI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(setupAction =>
            {

                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = " Portal",
                        Version = "1",
                        Description = "Portal to manage the  Product of ."
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = $"Input your Bearer token in this format - Bearer token to access this API",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }, new List<string>()
                    },
                });
            });

        }

        public static void AddVersion(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        public static void AddFluentControllers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllersWithViews()
                .AddNewtonsoftJson(ops => { ops.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; })
                .AddFluentValidation();
            serviceCollection.AddRazorPages();

            serviceCollection.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
                apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
                {
                    return new BadRequestObjectResult(new
                    {
                        Succeeded = false,
                        Code = 400,
                        Message = "Validation Error",
                        Errors = actionContext.ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                    });
                });
        }

        //public static void AddHTTPPolicies(this IServiceCollection serviceCollection, IConfiguration configuration)
        //{
        //    var policyConfigs = new HttpClientPolicyConfiguration();
        //    configuration.Bind("HttpClientPolicies", policyConfigs);

        //    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(policyConfigs.RetryTimeoutInSeconds));

        //    var retryPolicy = HttpPolicyExtensions
        //        .HandleTransientHttpError()
        //        .OrResult(r => r.StatusCode == HttpStatusCode.NotFound)
        //        .WaitAndRetryAsync(policyConfigs.RetryCount, _ => TimeSpan.FromMilliseconds(policyConfigs.RetryDelayInMs));

        //    var circuitBreakerPolicy = HttpPolicyExtensions
        //       .HandleTransientHttpError()
        //       .CircuitBreakerAsync(policyConfigs.MaxAttemptBeforeBreak, TimeSpan.FromSeconds(policyConfigs.BreakDurationInSeconds));

        //    var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

        //    //Register a Typed Instance of HttpClientFactory for a Protected Resource
        //    //More info see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0

        //    serviceCollection.AddHttpClient<IClientFactory, ClientFactory>(client =>
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    })
        //    .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
        //    .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
        //    .AddPolicyHandler(timeoutPolicy)
        //    .AddPolicyHandler(circuitBreakerPolicy);
        //}

        //public static void AddHealthCheck(this IServiceCollection serviceCollection, AppSettings appSettings, IConfiguration configuration)
        //{
        //    serviceCollection.AddHealthChecks()
        //        .AddUrlGroup(new Uri(appSettings.ApplicationDetail.ContactWebsite), name: "My personal website", failureStatus: HealthStatus.Degraded)
        //        .AddSqlServer(configuration.GetConnectionString("SQLDBConnectionString"));

        //    serviceCollection.AddHealthChecksUI(setupSettings: setup =>
        //    {
        //        setup.AddHealthCheckEndpoint("Basic Health Check", $"/healthz");
        //    }).AddInMemoryStorage();
        //}

        public static void AddRequestRateLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            serviceCollection.AddOptions();
            // needed to store rate limit counters and ip rules
            serviceCollection.AddMemoryCache();

            //load general configuration from appsettings.json
            serviceCollection.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            serviceCollection.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            serviceCollection.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            serviceCollection.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
