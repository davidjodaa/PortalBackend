using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.Contract;
using PortalBackend.Service.CustomAttributes;
using PortalBackend.Service.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace PortalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(SessionAttributeFilter))]
    public class AuditController : Controller
    {
        private readonly IAuditService _AuditService;
        public AuditController(IAuditService auditService)
        {
            _AuditService = auditService;
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet("getUserManagementReport")]
        public async Task<IActionResult> GetUserManagementReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _AuditService.GetUserManagementReport(queryParameters));
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost("downloadUserManagementReport")]
        public async Task<IActionResult> DownloadUserManagementReport(ReportRequest request)
        {
            string reportName = $"UserMgtAudit_{Guid.NewGuid():N}.xlsx";

            return File(await _AuditService.DownloadUserManagementReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("getPinManagementReport")]
        public async Task<IActionResult> GetPinManagementReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _AuditService.GetPinManagementReport(queryParameters));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("downloadPinManagementReport")]
        public async Task<IActionResult> DownloadPinManagementReport(ReportRequest request)
        {
            string reportName = $"PinMgtAudit_{Guid.NewGuid():N}.xlsx";

            return File(await _AuditService.DownloadPinManagementReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet("getProfileManagementReport")]
        public async Task<IActionResult> GetProfileManagementReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _AuditService.GetProfileManagementReport(queryParameters));
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost("downloadProfileManagementReport")]
        public async Task<IActionResult> DownloadProfileManagementReport(ReportRequest request)
        {
            string reportName = $"ProfileMgtAudit_{Guid.NewGuid():N}.xlsx";

            return File(await _AuditService.DownloadProfileManagementReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("getDeviceManagementReport")]
        public async Task<IActionResult> GetDeviceManagementReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _AuditService.GetDeviceManagementReport(queryParameters));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("downloadDeviceManagementReport")]
        public async Task<IActionResult> DownloadDeviceManagementReport(ReportRequest request)
        {
            string reportName = $"DeviceMgtAudit_{Guid.NewGuid():N}.xlsx";

            return File(await _AuditService.DownloadDeviceManagementReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }
    }
}
