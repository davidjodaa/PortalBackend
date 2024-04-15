using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class CreatePinApiRequest
    {
        public string channel { get; set; }
        public string customer_id { get; set; }
        public string account_no { get; set; }
        public string user_id { get; set; }
        public string pin { get; set; }
        public string reference { get; set; }
        public string hash_signature { get; set; } = "839595";
    }
}
