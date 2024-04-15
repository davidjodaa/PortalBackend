using PortalBackend.Domain.Common;
using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Entities;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Persistence;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class ReportService : IReportService
    {
        private readonly IApplicationDbContext _context;

        public ReportService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<AcctOpeningReport>>> GetAccountOpeningReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<AcctOpeningReport> pagedData = await _context.UserDevice
                .Where(x => x.CreatedDate >= StartDate.Date && x.CreatedDate <= EndDate.Date.AddDays(1))
                .Select(x => new AcctOpeningReport()
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.CustomerId,
                    DeviceStatus = x.DeviceStatus,
                    PhoneNumber = x.PhoneNumber,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No account opened during the time period.");
            }

            List<AcctOpeningReport> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<AcctOpeningReport>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved account opening report");
        }

        public async Task<byte[]> DownloadAccountOpeningReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<AcctOpeningReport> pagedData = await _context.UserDevice
                .Where(x => x.CreatedDate >= StartDate.Date && x.CreatedDate <= EndDate.Date.AddDays(1))
                .Select(x => new AcctOpeningReport()
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.CustomerId,
                    DeviceStatus = x.DeviceStatus,
                    PhoneNumber = x.PhoneNumber,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No account opened during the time period.");
            }

            return ExporttoExcel(pagedData, reportName);
        }

        public async Task<PagedResponse<List<TransactionReport>>> GetTransactionReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<TransactionReport> pagedData = await _context.TransferLog
                .Where(x => x.TransTime >= StartDate && x.TransTime <= EndDate.AddDays(1))
                .Select(x => new TransactionReport()
                {
                    Amount = x.Amount,
                    BankCode = x.BankCode,
                    DestinationAccount = x.DestinationAccount,
                    Narration = x.Narration,
                    OriginatingRef = x.OriginatingRef,
                    ProcessorRef = x.ProcessorRef,
                    ReceiverName = x.ReceiverName,
                    SenderName = x.SenderName,
                    SourceAccount = x.SourceAccount,
                    TransResponseCode = x.TransResponseCode,
                    TransStatus = x.TransStatus,
                    TransTime = x.TransTime,
                    TransType = x.TransType,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.TransTime)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No transactions during the time period.");
            }

            List<TransactionReport> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<TransactionReport>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved transaction report");
        }

        public async Task<byte[]> DownloadTransactionReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<TransactionReport> pagedData = await _context.TransferLog
                .Where(x => x.TransTime >= StartDate.Date && x.TransTime <= EndDate.Date.AddDays(1))
                .Select(x => new TransactionReport()
                {
                    Amount = x.Amount,
                    BankCode = x.BankCode,
                    DestinationAccount = x.DestinationAccount,
                    Narration = x.Narration,
                    OriginatingRef = x.OriginatingRef,
                    ProcessorRef = x.ProcessorRef,
                    ReceiverName = x.ReceiverName,
                    SenderName = x.SenderName,
                    SourceAccount = x.SourceAccount,
                    TransResponseCode = x.TransResponseCode,
                    TransStatus = x.TransStatus,
                    TransTime = x.TransTime,
                    TransType = x.TransType,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.TransTime)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No transactions during the time period.");
            }

            return ExporttoExcel(pagedData, reportName);
        }

        private static byte[] ExporttoExcel<T>(List<T> table, string filename)
        {
            using ExcelPackage pack = new();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light1);
            return pack.GetAsByteArray();
        }
    }
}
