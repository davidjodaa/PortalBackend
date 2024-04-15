using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class GetCustAcctInfoResponse
    {
        public string AccountNo { get; set; }
        public string CustNo { get; set; }
        public string Currency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string AccountName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNos { get; set; }
        public string Fax { get; set; }
        public DateTime Dob { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AccountType { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal BookBalance { get; set; }
        public bool IsActive { get; set; }
        public bool IsDebitAllowed { get; set; }
        public bool IsCreditAllowed { get; set; }
        public bool IsStaff { get; set; }
        public bool IsDualSignatory { get; set; }
        public object Signatories { get; set; }
        public bool IsJoint { get; set; }
        public bool HasChequeBookFacility { get; set; }
        public bool HasAtmFacility { get; set; }
        public bool HasPassbookFacility { get; set; }
        public DateTime DateOpened { get; set; }
        public decimal AmountBlocked { get; set; }
        public DateTime DateLastCredit { get; set; }
        public DateTime DateLastDebit { get; set; }
        public string AccountOfficer { get; set; }
        public string BusinessGroup { get; set; }
        public string BusinessSubSector { get; set; }
        public string CustCategory { get; set; }
        public string CustCategoryDesc { get; set; }
        public string ZipCode { get; set; }
        public string PndReason { get; set; }
        public bool HasSweep { get; set; }
        public decimal SweepAmount { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal UnclearedBalance { get; set; }
        public decimal OdLimit { get; set; }
        public string MsgId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
