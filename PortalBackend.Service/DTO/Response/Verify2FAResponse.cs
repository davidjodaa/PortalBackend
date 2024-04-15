using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class Verify2FAResponse
    {
        public string MsgId { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
    }
}
