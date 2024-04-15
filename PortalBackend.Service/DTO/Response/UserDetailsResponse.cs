using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class UserDetailsResponse 
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}
