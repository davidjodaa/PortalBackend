using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class ValidateProfileUserResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string CustomerId { get; set; }
        public string MobileNumber { get; set; }
        public string ProfileStatus { get; set; }
        public string DeviceStatus { get; set; }
    }
}
