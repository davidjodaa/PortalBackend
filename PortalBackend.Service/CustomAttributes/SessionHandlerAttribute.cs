using PortalBackend.Domain.Common;
using PortalBackend.Service.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.CustomAttributes
{
    public class SessionHandlerAttribute
    {
        private readonly RequestDelegate _next;
        private IAppSessionService _appSession;
        private readonly ILogger<SessionHandlerAttribute> _logger;

        public SessionHandlerAttribute(RequestDelegate requestDelegate, 
            ILogger<SessionHandlerAttribute> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IAppSessionService appSession)
        {
            _appSession = appSession;

            var username = context.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            var sessionId = context.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sessionId))
            {
                bool session = _appSession.ValidateSession(sessionId, username);
                if (session)
                {
                    await _next(context);
                }
                else
                {
                    await HandleResponseAsync(context, _logger);
                }
            }
            else
            {
                await HandleResponseAsync(context, _logger);
            }
        }
        private static Task HandleResponseAsync(HttpContext context, ILogger<SessionHandlerAttribute> logger)
        {
            var code = HttpStatusCode.Unauthorized; // 500 if unexpected
            string response = "Authorization Denied. Invalid session.";

            logger.LogError(response);

            var result = JsonConvert.SerializeObject(new Response<string>(response, false, (int)code) { });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
