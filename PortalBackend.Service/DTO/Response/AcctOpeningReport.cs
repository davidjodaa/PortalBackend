using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class AcctOpeningReport
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerId { get; set; }
        public string DeviceStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
    }
}
