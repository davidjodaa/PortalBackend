using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Domain.Settings
{
    public class ApiResourceUrls
    {
        public string MaintenanceEnquiry { get; set; }
        public string GetActiveDirectoryDetails { get; set; }
        public string ActiveDirectoryLogin { get; set; }
        public string ResetPin { get; set; }
        public string CreatePin { get; set; }
        public string QueryByUserId { get; set; }
        public string QueryByCustomerId { get; set; }
        public string GetCustAccountInfo { get; set; }
        public string SendSms { get; set; }
        public string SendEmail { get; set; }
        public string FindUser { get; set; }
        public string Verify2FA { get; set; }
        public string UpdateUser { get; set; }
        public string MaintenanceEnquiryToken { get; set; }
        public string EnableUser { get; set; }
        public string DisableUser { get; set; }
    }
}
