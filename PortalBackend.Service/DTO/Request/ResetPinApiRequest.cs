using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class ResetPinApiRequest
    {
        public string channel { get; set; }
        public string user_id { get; set; }
        public string customer_id { get; set; }
        public string reference { get; set; }
    }
}
