using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class UserResponse 
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string RoleId { get; set; }
    }
}
