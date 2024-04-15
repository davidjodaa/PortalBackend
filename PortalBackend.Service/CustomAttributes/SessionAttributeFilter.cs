using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.CustomAttributes
{
    public class SessionAttributeFilter
    {
        public void Configure(IApplicationBuilder builder)
        {
            builder.UseMiddleware<SessionHandlerAttribute>();
        }
    }
}
