using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class UpdateUserRequest
    {
        public string userId { get; set; }
        public string customerId { get; set; }
        public string status { get; set; }
    }
}
