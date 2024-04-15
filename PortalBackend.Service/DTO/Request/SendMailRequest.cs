using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class SendMailRequest
    {
        public string From { get; set; } = "noreply@plc.com";
        public string Recipient { get; set; }
        public string CopyAddress { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string DisplayName { get; set; }
    }
}
