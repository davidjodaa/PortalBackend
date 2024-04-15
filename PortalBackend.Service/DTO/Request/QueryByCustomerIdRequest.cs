using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class QueryByCustomerIdRequest
    {
        public string channel_code { get; set; }
        public string customerNumber { get; set; }
    }
}
