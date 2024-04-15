using System;
using System.Collections.Generic;

namespace PortalBackend.Service.DTO.Response
{
    public class AcctDetailsResponse
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string response_time { get; set; }
        public IEnumerable<GetCustomerAcctsDetailsResp> getcustomeracctsdetailsresp { get; set; }

        public class GetCustomerAcctsDetailsResp
        {
            public string branchCode { get; set; }
            public string accountNo { get; set; }
            public string accountName { get; set; }
            public string customerName { get; set; }
            public string currencyCode { get; set; }
            public string dateOpened { get; set; }
            public string custID { get; set; }
            public string customerNo { get; set; }
            public string accountStatus { get; set; }
            public string productCode { get; set; }
            public string accountClassType { get; set; }
            public string branchName { get; set; }
            public float availableBalance { get; set; }
            public string dateofbirth { get; set; }
            public string phone { get; set; }
            public string customerCategory { get; set; }
            public string accountOfficer { get; set; }
            public float amountCreditMTD { get; set; }
            public float amountCreditYTD { get; set; }
            public float amountDebitMTD { get; set; }
            public float amountDebitYTD { get; set; }
            public float amountHold { get; set; }
            public float amountLastCredit { get; set; }
            public float amountLastDebit { get; set; }
            public string aTMStatus { get; set; }
            public string bvn { get; set; }
            public float clearedBalance { get; set; }
            public float closingBalance { get; set; }
            public string comP_MIS_2 { get; set; }
            public string comP_MIS_4 { get; set; }
            public string comP_MIS_8 { get; set; }
            public string currency { get; set; }
            public string customerCategoryDesc { get; set; }
            public float daueLimit { get; set; }
            public string dAUEStartDate { get; set; }
            public string e_mail { get; set; }
            public float interestPaidYTD { get; set; }
            public float interestReceivedYTD { get; set; }
            public string lastCreditDate { get; set; }
            public float lastCreditInterestAccrued { get; set; }
            public float lastDebitInterestAccrued { get; set; }
            public string lastDebitDate { get; set; }
            public float acy_accrued_dr_ic { get; set; }
            public string lastMaintainedBy { get; set; }
            public string maintenanceAuthorizedBy { get; set; }
            public int lastSerialofCheque { get; set; }
            public int lastUsedChequeNo { get; set; }
            public float netBalance { get; set; }
            public float oDLimit { get; set; }
            public float openingBalance { get; set; }
            public string productName { get; set; }
            public string profitCenter { get; set; }
            public float serviceChargeYTD { get; set; }
            public string signatory { get; set; }
            public string staff { get; set; }
            public string strAdd1 { get; set; }
            public string strAdd2 { get; set; }
            public string strAdd3 { get; set; }
            public string strCity { get; set; }
            public string strCountry { get; set; }
            public string strState { get; set; }
            public string strZip { get; set; }
            public float taxAccrued { get; set; }
            public float unavailableBalance { get; set; }
            public float unclearedBalance { get; set; }
            public string customersegment { get; set; }
            public string pndReasonANDCode { get; set; }
            public string tierDetails { get; set; }
            public float hasSweep { get; set; }
            public float sweepAmt { get; set; }
            public string sweepData { get; set; }
            public string loanPrequalifyInfo { get; set; }
            public string migrateacctPrompt { get; set; }
            public string gender { get; set; }
            public string custAddress1 { get; set; }
            public string custAddress2 { get; set; }
            public string custAddress3 { get; set; }
            public string custAddress4 { get; set; }
            public string maritalStatus { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string customerType { get; set; }
            public string customerTIN { get; set; }
            public string cheque_book_facility { get; set; }
        }
    }
}
