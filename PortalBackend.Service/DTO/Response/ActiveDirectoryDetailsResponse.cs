using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class ActiveDirectoryDetailsResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ADLoginID { get; set; }
        public object AOAccountBranchCode { get; set; }
        public string AOBranch { get; set; }
        public string AccountOfficerName { get; set; }
        public string EmailAddress { get; set; }
        public object FirstLevelSupervisor { get; set; }
        public string MobileNo { get; set; }
        public object SecondLevelSupervisor { get; set; }
        public object StaffGrade { get; set; }
        public string StaffRole { get; set; }
        public string StaffGroup { get; set; }
        public string StaffOffice { get; set; }
        public string StaffBranch { get; set; }
        public string StaffLocation { get; set; }
    }
}
