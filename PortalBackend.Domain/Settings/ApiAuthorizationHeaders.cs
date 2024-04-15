using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Domain.Settings
{
    public class ApiAuthorizationHeaders
    {
        public string AuthKey { get; set; }
        public string EsbAuthKey { get; set; }
        public string EsbChannelCode { get; set; }
        public string BankApiChannelCode { get; set; }
        public string BankApiAuthKey { get; set; }
        public string TokenGroupName { get; set; }
    }
}
