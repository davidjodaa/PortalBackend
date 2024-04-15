using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class EditUserRequest
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string StaffGroup { get; set; }
        public string StaffBranch { get; set; }
        public string Role { get; set; }
    }
}
