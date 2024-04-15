using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class ValidateUnlockUserResponse
    {
        public string Uuid { get; set; }
        public DateTime? DateCreated { get; set; }
        public string NewUserId { get; set; }
        public string CurrentUserId { get; set; }
        public string NewUserName { get; set; }
        public string NewMobile { get; set; }
        public string NewEmail { get; set; }
        public string NewCustomerId { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentMobile { get; set; }
        public string CurrentEmail { get; set; }
        public string CurrentCustomerId { get; set; }
    }
}
