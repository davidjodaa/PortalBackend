using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class GetUserEnquiryDetailsRequest
    {
        public string AccountNo { get; set; }
        public string UserId { get; set; }
    }
}
