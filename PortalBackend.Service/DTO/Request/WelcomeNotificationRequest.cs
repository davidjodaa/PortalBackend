using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class WelcomeNotificationRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Branch { get; set; }
        public string Role { get; set; }
    }
}
