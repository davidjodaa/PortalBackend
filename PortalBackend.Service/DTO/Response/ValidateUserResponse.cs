using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class ValidateUserResponse
    {
        public string Username { get; set; }
        public object BranchCode { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; } 
        public string MobileNo { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
    }
}
