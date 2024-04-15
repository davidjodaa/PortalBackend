using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class AuthorizeRequestNotification
    {
        public string Authorizer { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string InitiatorsEmail { get; set; }
    }
}
