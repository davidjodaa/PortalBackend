using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PortalBackend.Domain.Common;
using PortalBackend.Domain.Settings;
using PortalBackend.Service.Contract;
using PortalBackend.Service.Implementation;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Net;
using PortalBackend.Persistence;
using Microsoft.AspNetCore.Identity;
using PortalBackend.Domain.Auth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PortalBackend.Service.CustomAttributes;

namespace PortalBackend.Service
{
    public static class DependencyInjection
    {
        public static void AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // or you can use assembly in Extension method in Infra layer with below command
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.Configure<ApiResourceUrls>(configuration.GetSection("ApiResourceUrls"));
            services.Configure<Secrets>(configuration.GetSection("Secrets"));
            services.Configure<ApiAuthorizationHeaders>(configuration.GetSection("ApiAuthorizationHeaders"));
            services.Configure<ActiveDirectorySettings>(configuration.GetSection("ADSettings"));
        }

        public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services
            services.AddTransient<IAccountService, AccountService>();
            #endregion
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            var code = HttpStatusCode.Unauthorized;
                            context.Response.StatusCode = (int)code;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not Authorized", false, (int)code));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            var code = HttpStatusCode.Forbidden;
                            context.Response.StatusCode = (int)code;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource", false, (int)code));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization();
            services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
        }
    }
}

