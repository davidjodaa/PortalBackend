using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Persistence;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class AuditService : IAuditService
    {
        private readonly IApplicationDbContext _context;

        public AuditService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<UserAuditResponse>>> GetUserManagementReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<UserAuditResponse> pagedData = await _context.PendingUserRequest
                .Where(x => x.CreatedAt >= StartDate.Date && x.CreatedAt <= EndDate.Date.AddDays(1))
                .Select(x => new UserAuditResponse()
                {
                    Id = x.Id,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DateAuthorized = x.DateAuthorized,
                    Branch = x.Branch,
                    InitiatingBranch = x.InitiatingBranch,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Group = x.Group,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    RoleId = x.RoleId,
                    RequestType = x.RequestType,
                    MobileNumber = x.MobileNumber,
                    UserName = x.UserName,
                    Name = x.Name
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No user management audit report during the time period.");
            }

            List<UserAuditResponse> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<UserAuditResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved user management audit report");
        }

        public async Task<byte[]> DownloadUserManagementReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<UserAuditResponse> pagedData = await _context.PendingUserRequest
                .Where(x => x.CreatedAt >= StartDate.Date && x.CreatedAt <= EndDate.Date.AddDays(1))
                .Select(x => new UserAuditResponse()
                {
                    Id = x.Id,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DateAuthorized = x.DateAuthorized,
                    Branch = x.Branch,
                    InitiatingBranch = x.InitiatingBranch,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Group = x.Group,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    RoleId = x.RoleId,
                    RequestType = x.RequestType,
                    MobileNumber = x.MobileNumber,
                    UserName = x.UserName,
                    Name = x.Name
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No account opened during the time period.");
            }

            return ExporttoExcel(pagedData, reportName);
        }

        public async Task<PagedResponse<List<PinAuditResponse>>> GetPinManagementReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<PinAuditResponse> pagedData = await _context.PinManagement
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new PinAuditResponse()
                {
                    AccountNo = x.AccountNo,
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    CustomerId = x.CustomerId,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    Mobile = x.Mobile,
                    UpdateType = x.UpdateType,
                    UserId = x.UserId,
                    UserStatus = x.UserStatus
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No pin management audit report during the time period.");
            }

            List<PinAuditResponse> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<PinAuditResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved pin management audit report");
        }

        public async Task<byte[]> DownloadPinManagementReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<PinAuditResponse> pagedData = await _context.PinManagement
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new PinAuditResponse()
                {
                    AccountNo = x.AccountNo,
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    CustomerId = x.CustomerId,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    Mobile = x.Mobile,
                    UpdateType = x.UpdateType,
                    UserId = x.UserId,
                    UserStatus = x.UserStatus
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No transactions during the time period.");
            }

            return ExporttoExcel(pagedData, reportName);
        }

        public async Task<PagedResponse<List<ProfileAuditResponse>>> GetProfileManagementReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<ProfileAuditResponse> pagedData = await _context.ProfileUpdate
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new ProfileAuditResponse()
                {
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    CustomerId = x.CustomerId,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    Mobile = x.Mobile,
                    ProfileStatus = x.ProfileStatus,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No profile update audit report during the time period.");
            }

            List<ProfileAuditResponse> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<ProfileAuditResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved profile update audit report");
        }

        public async Task<byte[]> DownloadProfileManagementReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<ProfileAuditResponse> pagedData = await _context.ProfileUpdate
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new ProfileAuditResponse()
                {
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    CustomerId = x.CustomerId,
                    DateInitiated = x.DateInitiated,
                    Email = x.Email,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    Mobile = x.Mobile,
                    ProfileStatus = x.ProfileStatus,
                    UserId = x.UserId
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No profile update audit report during the time period.");
            }

            return ExporttoExcel(pagedData, reportName);
        }

        public async Task<PagedResponse<List<DeviceAuditResponse>>> GetDeviceManagementReport(ReportQueryParameters queryParameters)
        {
            DateTime StartDate = string.IsNullOrEmpty(queryParameters.StartDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            List<DeviceAuditResponse> pagedData = await _context.DeviceUnlock
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new DeviceAuditResponse()
                {
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    DateInitiated = x.DateInitiated,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    CurrentUser = x.CurrentUser,
                    NewUser = x.NewUser
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No devince unlock audit report during the time period.");
            }

            List<DeviceAuditResponse> response = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            int totalRecords = pagedData.Count;

            return new PagedResponse<List<DeviceAuditResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved devince unlock audit report");
        }

        public async Task<byte[]> DownloadDeviceManagementReport(ReportRequest request, string reportName)
        {
            DateTime StartDate = string.IsNullOrEmpty(request.StartDate) ? DateTime.Now.Date : DateTime.Parse(request.StartDate, null);
            DateTime EndDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            List<DeviceAuditResponse> pagedData = await _context.DeviceUnlock
                .Where(x => x.CreatedAt >= StartDate && x.CreatedAt <= EndDate.AddDays(1))
                .Select(x => new DeviceAuditResponse()
                {
                    APIResponseCode = x.APIResponseCode,
                    APIResponseMessage = x.APIResponseMessage,
                    Authorizer = x.Authorizer,
                    AuthorizerEmail = x.AuthorizerEmail,
                    AuthorizersComment = x.AuthorizersComment,
                    AuthStatus = x.AuthStatus,
                    CreatedAt = x.CreatedAt,
                    DateAuthorized = x.DateAuthorized,
                    UpdatedAt = x.UpdatedAt,
                    DateInitiated = x.DateInitiated,
                    Id = x.Id,
                    InitiatingBranch = x.InitiatingBranch,
                    Initiator = x.Initiator,
                    InitiatorEmail = x.InitiatorEmail,
                    CurrentUser = x.CurrentUser,
                    NewUser = x.NewUser
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (pagedData.Count < 1)
            {
                throw new ApiException($"No devince unlock audit report during the time period.");
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
