using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.Contract;
using PortalBackend.Service.CustomAttributes;
using PortalBackend.Service.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using FluentValidation;

namespace PortalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(SessionAttributeFilter))]
    public class ReportController : Controller
    {
        private readonly IReportService _ReportService;
        private readonly IValidator<ReportRequest> _reportRequestValidator;
        public ReportController(IReportService ReportService,
            IValidator<ReportRequest> reportRequestValidator)
        {
            _ReportService = ReportService;
            _reportRequestValidator = reportRequestValidator;
        }
        [Authorize(Roles = "Administrator,Initiator,Authorizer")]
        [HttpGet("getAccountOpeningReport")]
        public async Task<IActionResult> GetAccountOpeningReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _ReportService.GetAccountOpeningReport(queryParameters));
        }
        [Authorize(Roles = "Administrator,Initiator,Authorizer")]
        [HttpPost("downloadAccountOpeningReport")]
        public async Task<IActionResult> DownloadAccountOpeningReport(ReportRequest request)
        {
            await _reportRequestValidator.ValidateAndThrowAsync(request);
            string reportName = $"AcctOpening_{Guid.NewGuid():N}.xlsx";

            return File(await _ReportService.DownloadAccountOpeningReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }

        [Authorize(Roles = "Administrator,Initiator,Authorizer")]
        [HttpGet("getTransactionReport")]
        public async Task<IActionResult> GetTransactionReport([FromQuery] ReportQueryParameters queryParameters)
        {
            return Ok(await _ReportService.GetTransactionReport(queryParameters));
        }

        [Authorize(Roles = "Administrator,Initiator,Authorizer")]
        [HttpPost("downloadTransactionReport")]
        public async Task<IActionResult> DownloadTransactionReport(ReportRequest request)
        {
            await _reportRequestValidator.ValidateAndThrowAsync(request);
            string reportName = $"AcctOpening_{Guid.NewGuid():N}.xlsx";

            return File(await _ReportService.DownloadTransactionReport(request, reportName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        }
    }
}
