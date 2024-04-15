using PortalBackend.Domain.Common;
using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Entities;
using PortalBackend.Domain.Enum;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Domain.Settings;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class ProfileUpdateService : IProfileUpdateService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAPIImplementation _apiCall;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;

        public ProfileUpdateService(IApplicationDbContext context,
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
        public async Task<Response<ValidateProfileUserResponse>> ValidateProfileUser(ValidateProfileUserRequest request)
        {
            // Get the user device currently tied to the user id in the request
            UserDevice userDevice = await _context.UserDevice
                .Where(d => d.UserId == request.UserId.ToLower())
                .FirstOrDefaultAsync();

            if (userDevice == null)
            {
                throw new ApiException($"An error occured. The user Id passed is not linked to any known device.");
            }

            // Get the user information for both the new user attempting to login and the current user as shown in the User Device table
            UserIdRequest queryRequest = new()
            {
                user_id = request.UserId
            };
            QueryUserResponse queryUserResponse = await _apiCall.QueryUserByUserId(queryRequest);

            if (queryUserResponse?.response_code != "00")
            {
                throw new ApiException($"An error occured while querying the User Id.");
            }

            ValidateProfileUserResponse response = _mapper.Map<ValidateProfileUserResponse>(queryUserResponse.response_data);

            if (queryUserResponse.response_data.user_status == UserAPIStatus.ACTIVE || string.IsNullOrEmpty(queryUserResponse.response_data.user_status))
            {
                response.ProfileStatus = "Enabled";
            }
            else
            {
                response.ProfileStatus = "Disabled";
            }
            response.DeviceStatus = userDevice.DeviceStatus == "1" ? "ENABLED" : "DISABLED";

            return new Response<ValidateProfileUserResponse>(response, $"Successfully validated profile update user with Id - {request.UserId}");
        }

        public async Task<Response<long>> AddProfileRequest(AddProfileRequest request)
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

            ProfileUpdate profileUpdate = _mapper.Map<ProfileUpdate>(request);
            profileUpdate.Initiator = username;
            profileUpdate.InitiatorEmail = email;
            profileUpdate.InitiatingBranch = branch;
            profileUpdate.DateInitiated = DateTime.Now;
            profileUpdate.AuthStatus = AuthStatus.Pending;

            // Convert the formfile to a bytes array
            // This should be extracted to an helper service
            Stream fileStream = request.SupportingDocument.OpenReadStream();
            byte[] byteData = new byte[fileStream.Length];
            fileStream.Read(byteData, 0, Convert.ToInt32(fileStream.Length));

            // Add the file and other properties to the entity
            profileUpdate.SupportingDocument = byteData;
            profileUpdate.FileExtension = Path.GetExtension(request.SupportingDocument.FileName);
            profileUpdate.FileName = request.SupportingDocument.FileName;

            await _context.ProfileUpdate.AddAsync(profileUpdate);
            await _context.SaveChangesAsync();

            InitiateRequestNotification initiateRequest = _mapper.Map<InitiateRequestNotification>(request);
            initiateRequest.Action = "Profile Update Request";
            initiateRequest.AuthorizersEmail = authorizer.Email;
            initiateRequest.Initiator = username;

            await _notificationService.InitiateRequestNotification(initiateRequest);

            return new Response<long>(profileUpdate.Id, message: $"Successfully initiated the Profile Request for User ID - {request.UserId}");
        }

        public async Task<Response<long>> AuthorizeProfileRequest(AuthRequest request)
        {
            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            ProfileUpdate profileUpdateRequest = await _context.ProfileUpdate
                .Where(pm => pm.Id == request.Id && pm.AuthStatus == AuthStatus.Pending && pm.Authorizer == username)
                .FirstOrDefaultAsync();

            if (profileUpdateRequest == null)
            {
                throw new ApiException($"No Profile request found for ID - {request.Id}.");
            }

            // Update the Profile Management table
            profileUpdateRequest.Authorizer = username;
            profileUpdateRequest.DateAuthorized = DateTime.Now;
            profileUpdateRequest.AuthStatus = request.AuthStatus;
            profileUpdateRequest.AuthorizersComment = request.AuthorizersComment;
            profileUpdateRequest.AuthorizerEmail = email;

            // If unlock request is approved then perform the necessary operation
            //ProfileApiResponse pinApiResponse = new ProfileApiResponse();
            if (request.AuthStatus == AuthStatus.Approved)
            {
                // Update the device status on the user devices table
                UserDevice userDevice = await _context.UserDevice
                    .Where(d => d.UserId.ToLower() == profileUpdateRequest.UserId.ToLower())
                    .FirstOrDefaultAsync();

                if (userDevice == null)
                {
                    throw new ApiException($"No user device found for the user.");
                }

                UpdateUserRequest updateUserRequest = _mapper.Map<UpdateUserRequest>(profileUpdateRequest);

                UserIdRequest userIdRequest = new UserIdRequest()
                {
                    user_id = profileUpdateRequest.UserId
                };

                UserApiResponse userApiResponse = new UserApiResponse();

                switch (profileUpdateRequest.ProfileStatus)
                {
                    case ProfileStatus.Enable:
                        userApiResponse = await EnableUser(userIdRequest);
                        userDevice.DeviceStatus = "1";
                        updateUserRequest.status = UserAPIStatus.ACTIVE;
                        break;
                    case ProfileStatus.Disable:
                        userApiResponse = await DisableUser(userIdRequest);
                        userDevice.DeviceStatus = "0";
                        updateUserRequest.status = UserAPIStatus.INACTIVE;
                        break;
                    default:
                        break;
                }

                // Update the device unlock table with additional details
                profileUpdateRequest.APIResponseCode = userApiResponse.response_code;
                profileUpdateRequest.APIResponseMessage = userApiResponse.response_message;

                _context.UserDevice.Update(userDevice);
            }

            EntityEntry<ProfileUpdate> response = _context.ProfileUpdate.Update(profileUpdateRequest);
            await _context.SaveChangesAsync();

            AuthorizeRequestNotification authorizeRequest = _mapper.Map<AuthorizeRequestNotification>(profileUpdateRequest);
            authorizeRequest.Action = "Profile Update Request";
            authorizeRequest.Status = request.AuthStatus.ToString();

            return new Response<long>(request.Id, $"Successfully authorized profile update request with Id - {request.Id}");
        }

        public PagedResponse<List<PendingProfileResponse>> GetPendingProfileRequests(ProfileQueryParameters queryParameters)
        {
            var username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;

            IQueryable<PendingProfileUpdate> pagedData = _context.ProfileUpdate
                .Where(x => x.AuthStatus == AuthStatus.Pending && x.Authorizer == username)
                .Select(x => new PendingProfileUpdate
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CustomerId = x.CustomerId,
                    Mobile = x.Mobile,
                    Email = x.Email,
                    ProfileStatus = x.ProfileStatus,
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
                   || x.CustomerId.ToLower().Contains(query.ToLower())
                   || x.Email.ToLower().Contains(query.ToLower())
                   || x.Initiator.ToLower().Contains(query.ToLower())
                   || x.Mobile.ToLower().Contains(query.ToLower()));
            }

            List<PendingProfileUpdate> pendingProfileList = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            List<PendingProfileResponse> response = _mapper.Map<List<PendingProfileUpdate>, List<PendingProfileResponse>>(pendingProfileList);

            int totalRecords = pagedData.ToList().Count;

            return new PagedResponse<List<PendingProfileResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved pending profile update requests.");
        }

        public async Task<Response<PendingProfileResponse>> GetPendingProfileRequestById(long id)
        {
            ProfileUpdate profileUpdate = await _context.ProfileUpdate.FindAsync(id);

            if (profileUpdate == null)
            {
                throw new ApiException($"No profile update request found for ID - {id}.");
            }

            PendingProfileResponse response = _mapper.Map<ProfileUpdate, PendingProfileResponse>(profileUpdate);
            response.FileDocument = Utils.ConvertToBase64DataUrl(profileUpdate.SupportingDocument, profileUpdate.FileExtension);
            response.FileName = profileUpdate.FileName;

            return new Response<PendingProfileResponse>(response, $"Successfully retrieved pending profile update request details with Id - {id}");
        }

        /**
         * Private Functions for internal operations
         **/
        private async Task<UserApiResponse> EnableUser(UserIdRequest request)
        {
            // Call the API to disable the profile from Bank API
            UserApiResponse response = await _apiCall.EnableUser(request);

            if (response?.response_code != "00")
            {
                throw new ApiException($"An error occured while updating the user's status on the core banking.");
            }

            return response;
        }

        private async Task<UserApiResponse> DisableUser(UserIdRequest request)
        {
            // Call the API to disable the profile from Bank API
            UserApiResponse response = await _apiCall.DisableUser(request);

            if (response?.response_code != "00")
            {
                throw new ApiException($"An error occured while updating the user's status on the core banking.");
            }

            return response;
        }
    }
}
