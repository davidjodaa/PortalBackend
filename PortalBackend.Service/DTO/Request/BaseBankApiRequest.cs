using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class BaseBankApiRequest
    {
        public string MsgId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Processor { get; set; }
        public string Channel { get; set; }
    }
}
