using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IReportService
    {
        Task<PagedResponse<List<AcctOpeningReport>>> GetAccountOpeningReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadAccountOpeningReport(ReportRequest request, string reportName);
        Task<PagedResponse<List<TransactionReport>>> GetTransactionReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadTransactionReport(ReportRequest request, string reportName);
    }
}
