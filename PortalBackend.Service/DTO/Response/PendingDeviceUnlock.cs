using PortalBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class PendingDeviceUnlock
    {
        public long Id { get; set; }
        public string NewUser { get; set; }
        public string CurrentUser { get; set; }
        public string Uuid { get; set; }
        public string Initiator { get; set; }
        public string InitiatorEmail { get; set; }
        public DateTime DateInitiated { get; set; }
        public string InitiatingBranch { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
