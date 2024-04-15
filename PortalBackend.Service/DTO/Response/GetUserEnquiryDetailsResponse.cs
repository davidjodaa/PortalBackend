using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class GetUserEnquiryDetailsResponse
    {
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string UserRole { get; set; }
        public string UserStatus { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ProfileStatus { get; set; }
        public string AppVersion { get; set; }
        public string DeviceType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
