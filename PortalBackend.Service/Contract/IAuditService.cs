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
    public interface IAuditService
    {
        Task<PagedResponse<List<UserAuditResponse>>> GetUserManagementReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadUserManagementReport(ReportRequest request, string reportName);
        Task<PagedResponse<List<PinAuditResponse>>> GetPinManagementReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadPinManagementReport(ReportRequest request, string reportName);
        Task<PagedResponse<List<ProfileAuditResponse>>> GetProfileManagementReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadProfileManagementReport(ReportRequest request, string reportName);
        Task<PagedResponse<List<DeviceAuditResponse>>> GetDeviceManagementReport(ReportQueryParameters queryParameters);
        Task<byte[]> DownloadDeviceManagementReport(ReportRequest request, string reportName);
    }
}
