using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Domain.Settings
{
    public class Secrets
    {
        public string adUserKey { get; set; }
        public string adUserChannel { get; set; }
        public string adSecretKey { get; set; }
        public string adAuthorizationHeader { get; set; }
        public string adUserGroup { get; set; }
        public bool EnableToken { get; set; }
        public bool EnableSelfAuthCheck { get; set; }
        public string[] Cors { get; set; }
    }
}
