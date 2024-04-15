using PortalBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class PendingProfileUpdate
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public ProfileStatus ProfileStatus { get; set; }
        public string Initiator { get; set; }
        public string InitiatorEmail { get; set; }
        public DateTime DateInitiated { get; set; }
        public string InitiatingBranch { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
