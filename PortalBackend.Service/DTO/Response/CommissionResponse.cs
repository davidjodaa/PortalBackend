using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class CommissionResponse
    {
        public long Id { get; set; }
        public int CommisionPercent { get; set; }
        public int MaxAmount { get; set; }
        public int MinAmount { get; set; }
        public decimal? Charges { get; set; }
        public string TransactionType { get; set; }
        public decimal? VATRate { get; set; }
    }
}
