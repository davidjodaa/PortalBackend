using PortalBackend.Domain.Common;
using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Entities;
using PortalBackend.Domain.Enum;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Persistence;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using PortalBackend.Service.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class UnlockService : IUnlockService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAPIImplementation _apiCall;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;

        public UnlockService(IApplicationDbContext context,
            IAPIImplementation apiCall,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IAccountService accountService,
            INotificationService notificationService)
        {
            _context = context;
            _apiCall = apiCall;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _accountService = accountService;
            _notificationService = notificationService;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * 
        **/
        public async Task<Response<ValidateUnlockUserResponse>> ValidateUnlockUser(ValidateUnlockUserRequest request)
        {
            // Get the new user's last known login session from the session service log to know the device he is trying to unlock
            // Throw an exception if there is no known session log in the database
            SessionServiceLog sessionServiceLog = await _context.SessionServiceLog
                .Where(s => s.UserId == request.UserId.ToLower())
                .OrderByDescending(s => s.CreatedDate)
                .FirstOrDefaultAsync();

            if (sessionServiceLog == null)
            {
                throw new ApiException($"An error occured. Kindly advise the customer to attempt to login on .");
            }

            // Get the user device currently tied to the UUID found in the session service log
            UserDevice userDevice = await _context.UserDevice
                .Where(d => d.Uuid == sessionServiceLog.Uuid)
                .FirstOrDefaultAsync();

            if (userDevice == null)
            {
                throw new ApiException($"An error occured. The device is not linked to any known user.");
            }

            // Get the user information for both the new user attempting to login and the current user as shown in the User Device table
            UserIdRequest newQueryRequest = new()
            {
                user_id = sessionServiceLog.UserId
            };
            QueryUserResponse newQueryUserResponse = await _apiCall.QueryUserByUserId(newQueryRequest);

            UserIdRequest currentQueryRequest = new()
            {
                user_id = userDevice.UserId
            };
            QueryUserResponse currentQueryUserResponse = await _apiCall.QueryUserByUserId(currentQueryRequest);

            ValidateUnlockUserResponse response = _mapper.Map<ValidateUnlockUserResponse>(userDevice);

            response.NewUserId = sessionServiceLog.UserId;
            response.DateCreated = sessionServiceLog.CreatedDate;

            response.NewCustomerId = newQueryUserResponse?.response_data?.customer_id ?? "";
            response.NewEmail = newQueryUserResponse?.response_data?.email ?? "";
            response.NewUserName = newQueryUserResponse?.response_data?.user_name ?? "";
            response.NewMobile = newQueryUserResponse?.response_data?.customer_id ?? "";

            response.CurrentCustomerId = currentQueryUserResponse?.response_data?.customer_id ?? "";
            response.CurrentEmail = currentQueryUserResponse?.response_data?.email ?? "";
            response.CurrentMobile = currentQueryUserResponse?.response_data?.mobile ?? "";
            response.CurrentUserName = currentQueryUserResponse?.response_data?.user_name ?? "";

            return new Response<ValidateUnlockUserResponse>(response, $"Successfully validated unlock user with Id - {request.UserId}");
        }

        public async Task<Response<long>> AddUnlockRequest(AddUnlockRequest request)
        {
            // Validate the authorizer
            AuthorizerResponse authorizer = await _accountService.GetUserInRole(request.Authorizer, RoleConstants.Authorizer);

            if (authorizer == null)
            {
                throw new ApiException($"An error occured while validating the authorizer.");
            }

            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            DeviceUnlock deviceUnlock = _mapper.Map<DeviceUnlock>(request);
            deviceUnlock.Initiator = username;
            deviceUnlock.InitiatorEmail = email;
            deviceUnlock.InitiatingBranch = branch;
            deviceUnlock.DateInitiated = DateTime.Now;
            deviceUnlock.AuthStatus = AuthStatus.Pending;

            // Convert the formfile to a bytes array
            // This should be extracted to an helper service
            Stream fileStream = request.SupportingDocument.OpenReadStream();
            byte[] byteData = new byte[fileStream.Length];
            fileStream.Read(byteData, 0, Convert.ToInt32(fileStream.Length));

            // Add the file and other properties to the entity
            deviceUnlock.SupportingDocument = byteData;
            deviceUnlock.FileExtension = Path.GetExtension(request.SupportingDocument.FileName);
            deviceUnlock.FileName = request.SupportingDocument.FileName;

            await _context.DeviceUnlock.AddAsync(deviceUnlock);
            await _context.SaveChangesAsync();

            InitiateRequestNotification initiateRequest = _mapper.Map<InitiateRequestNotification>(request);
            initiateRequest.Action = "Device Unlock Request";
            initiateRequest.AuthorizersEmail = authorizer.Email;
            initiateRequest.Initiator = username;

            await _notificationService.InitiateRequestNotification(initiateRequest);

            return new Response<long>(deviceUnlock.Id, message: $"Successfully initiated the Unlock Request for User ID - {request.NewUser}");
        }

        public async Task<Response<long>> AuthorizeUnlockRequest(AuthRequest request)
        {
            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            DeviceUnlock deviceUnlockRequest = await _context.DeviceUnlock
                .Where(pm => pm.Id == request.Id && pm.AuthStatus == AuthStatus.Pending && pm.Authorizer == username)
                .FirstOrDefaultAsync();

            if (deviceUnlockRequest == null)
            {
                throw new ApiException($"No Unlock request found for ID - {request.Id}.");
            }

            // Update the Unlock Management table
            deviceUnlockRequest.Authorizer = username;
            deviceUnlockRequest.DateAuthorized = DateTime.Now;
            deviceUnlockRequest.AuthStatus = request.AuthStatus;
            deviceUnlockRequest.AuthorizersComment = request.AuthorizersComment;
            deviceUnlockRequest.AuthorizerEmail = email;

            // If unlock request is approved then perform the necessary operation
            //UnlockApiResponse pinApiResponse = new UnlockApiResponse();
            if (request.AuthStatus == AuthStatus.Approved)
            {
                // fetch the user device tied to the UUID saved in the unlock request
                UserDevice userDevice = await _context.UserDevice
                    .Where(d => d.UserId.ToLower() == deviceUnlockRequest.CurrentUser.ToLower())
                    .FirstOrDefaultAsync();

                if (userDevice == null)
                {
                    throw new ApiException($"No user device found for the current user.");
                }

                userDevice.Uuid = string.Empty;

                // Update the device unlock table with additional details
                deviceUnlockRequest.APIResponseCode = "00";
                deviceUnlockRequest.APIResponseMessage = "SUCCESSFULLY UNLOCKED";

                _context.UserDevice.Update(userDevice);
            }

            EntityEntry<DeviceUnlock> response = _context.DeviceUnlock.Update(deviceUnlockRequest);
            await _context.SaveChangesAsync();

            AuthorizeRequestNotification authorizeRequest = _mapper.Map<AuthorizeRequestNotification>(deviceUnlockRequest);
            authorizeRequest.Action = "Device Unlock Request";
            authorizeRequest.Status = request.AuthStatus.ToString();

            await _notificationService.AuthorizeNotification(authorizeRequest);

            return new Response<long>(request.Id, $"Successfully authorized Device Unlock request with Id - {request.Id}");
        }

        public PagedResponse<List<PendingUnlockResponse>> GetPendingUnlockRequests(UnlockQueryParameters queryParameters)
        {
            var username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;

            IQueryable<PendingDeviceUnlock> pagedData = _context.DeviceUnlock
                .Where(x => x.AuthStatus == AuthStatus.Pending && x.Authorizer == username)
                .Select(x => new PendingDeviceUnlock
                {
                    Id = x.Id,
                    NewUser = x.NewUser,
                    CurrentUser = x.CurrentUser,
                    Uuid = x.Uuid,
                    Initiator = x.Initiator,
                    InitiatingBranch = x.InitiatingBranch,
                    InitiatorEmail = x.InitiatorEmail,
                    DateInitiated = x.DateInitiated,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if (!pagedData.Any())
            {
                throw new ApiException($"No pending pin requests found with the requested parameters.");
            }

            string query = queryParameters.Query;

            // Check if there is a query and apply it
            if (!string.IsNullOrEmpty(query))
            {
                pagedData = pagedData.Where(x => x.Id.Equals(query.ToLower())
                   || x.CurrentUser.ToLower().Contains(query.ToLower())
                   || x.NewUser.ToLower().Contains(query.ToLower())
                   || x.Initiator.ToLower().Contains(query.ToLower())
                   || x.Uuid.ToLower().Contains(query.ToLower()));
            }

            List<PendingDeviceUnlock> pendingUnlockList = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            List<PendingUnlockResponse> response = _mapper.Map<List<PendingDeviceUnlock>, List<PendingUnlockResponse>>(pendingUnlockList);

            int totalRecords = pagedData.ToList().Count;

            return new PagedResponse<List<PendingUnlockResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved pending unlock requests.");
        }

        public async Task<Response<PendingUnlockResponse>> GetPendingUnlockRequestById(long id)
        {
            DeviceUnlock deviceUnlock = await _context.DeviceUnlock.FindAsync(id);

            if (deviceUnlock == null)
            {
                throw new ApiException($"No pin request found for Unlock ID - {id}.");
            }

            PendingUnlockResponse response = _mapper.Map<DeviceUnlock, PendingUnlockResponse>(deviceUnlock);
            response.FileDocument = Utils.ConvertToBase64DataUrl(deviceUnlock.SupportingDocument, deviceUnlock.FileExtension);
            response.FileName = deviceUnlock.FileName;

            return new Response<PendingUnlockResponse>(response, $"Successfully retrieved pending device unlock request details with Id - {id}");
        }

        /**
         * Private Functions for internal operations
         **/
    }
}
