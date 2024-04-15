using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class TransactionReport
    {
        public string UserId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public decimal? Amount { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public string Narration { get; set; }
        public string TransStatus { get; set; }
        public string TransType { get; set; }
        public string BankCode { get; set; }
        public DateTime? TransTime { get; set; }
        public string TransResponseCode { get; set; }
        public string OriginatingRef { get; set; }
        public string ProcessorRef { get; set; }
    }
}
