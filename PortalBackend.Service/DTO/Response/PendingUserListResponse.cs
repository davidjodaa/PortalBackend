using PortalBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class PendingUserListResponse
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string DefaultRole { get; set; }
        public UserRequestType RequestType { get; set; }
        public string Initiator { get; set; }
        public string InitiatorEmail { get; set; }
        public DateTime DateInitiated { get; set; }
        public string InitiatingBranch { get; set; }
    }
}
