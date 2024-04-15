using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class InitiateRequestNotification
    {
        public string Initiator { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string AuthorizersEmail { get; set; }
    }
}
