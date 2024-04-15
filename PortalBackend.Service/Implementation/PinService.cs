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
    public class PinService : IPinService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAPIImplementation _apiCall;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApiAuthorizationHeaders _apiAuthorizationHeaders;
        private readonly Secrets _secrets;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;

        public PinService(IApplicationDbContext context,
            IAPIImplementation apiCall,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IOptions<ApiAuthorizationHeaders> apiAuthorizationHeaders,
            IOptions<Secrets> secrets,
            IAccountService accountService,
            INotificationService notificationService)
        {
            _context = context;
            _apiCall = apiCall;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _apiAuthorizationHeaders = apiAuthorizationHeaders.Value;
            _secrets = secrets.Value;
            _accountService = accountService;
            _notificationService = notificationService;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * 
        **/
        public async Task<Response<long>> AddPinRequest(AddPinRequest request)
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

            // Get all pending Pin requests for this user id and update them
            List<PinManagement> pendingPinRequests = await _context.PinManagement
                .Where(pm => pm.UserId == request.UserId && pm.AuthStatus == AuthStatus.Pending)
                .ToListAsync();

            foreach (PinManagement pinRequest in pendingPinRequests)
            {
                pinRequest.AuthStatus = AuthStatus.Reinitiated;
                pinRequest.AuthorizersComment = "This request was reinitiated";
            }

            PinManagement pinManagement = _mapper.Map<PinManagement>(request);
            pinManagement.Initiator = username;
            pinManagement.InitiatorEmail = email;
            pinManagement.InitiatingBranch = branch;
            pinManagement.DateInitiated = DateTime.Now;
            pinManagement.AuthStatus = AuthStatus.Pending;

            // Convert the formfile to a bytes array
            // This should be extracted to an helper service
            Stream fileStream = request.SupportingDocument.OpenReadStream();
            byte[] byteData = new byte[fileStream.Length];
            fileStream.Read(byteData, 0, Convert.ToInt32(fileStream.Length));

            // Add the file and other properties to the entity
            pinManagement.SupportingDocument = byteData;
            pinManagement.FileExtension = Path.GetExtension(request.SupportingDocument.FileName);
            pinManagement.FileName = request.SupportingDocument.FileName;

            await _context.PinManagement.AddAsync(pinManagement);
            await _context.SaveChangesAsync();

            InitiateRequestNotification initiateRequest = _mapper.Map<InitiateRequestNotification>(request);
            initiateRequest.Action = "Pin Request";
            initiateRequest.AuthorizersEmail = authorizer.Email;
            initiateRequest.Initiator = username;

            await _notificationService.InitiateRequestNotification(initiateRequest);

            return new Response<long>(pinManagement.Id, message: $"Successfully initiated the Pin Request for User ID - {request.UserId}");
        }

        public async Task<Response<long>> AuthorizePinRequest(AuthRequest request)
        {
            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            PinManagement pinManagement = await _context.PinManagement
                .Where(pm => pm.Id == request.Id && pm.AuthStatus == AuthStatus.Pending && pm.Authorizer == username)
                .FirstOrDefaultAsync();

            if (pinManagement == null)
            {
                throw new ApiException($"No Pin request found for ID - {request.Id}.");
            }

            if (username == pinManagement.Initiator && _secrets.EnableSelfAuthCheck)
            {
                throw new ApiException($"Self authorization is not allowed.");
            }

            // Update the Pin Management table
            pinManagement.Authorizer = username;
            pinManagement.DateAuthorized = DateTime.Now;
            pinManagement.AuthStatus = request.AuthStatus;
            pinManagement.AuthorizersComment = request.AuthorizersComment;
            pinManagement.AuthorizerEmail = email;

            // If pin is authorized then perform the necessary operation
            BankApiResponse pinApiResponse = new();
            if (request.AuthStatus == AuthStatus.Approved)
            {
                pinApiResponse = pinManagement.UpdateType switch
                {
                    PinUpdateType.CreatePin => await CreatePin(pinManagement),
                    PinUpdateType.ResetPin => await ResetPin(pinManagement),
                    _ => throw new ApiException($"Invalid Pin update type passed."),
                };
                if (pinApiResponse?.response_code != "00")
                {
                    throw new ApiException($"An error occured while calling external services.");
                }
                pinManagement.APIResponseCode = pinApiResponse.response_code;
                pinManagement.APIResponseMessage = pinApiResponse.response_message;
            }

            EntityEntry<PinManagement> response = _context.PinManagement.Update(pinManagement);
            await _context.SaveChangesAsync();

            AuthorizeRequestNotification authorizeRequest = _mapper.Map<AuthorizeRequestNotification>(pinManagement);
            authorizeRequest.Action = "Pin Request";
            authorizeRequest.Status = request.AuthStatus.ToString();

            await _notificationService.AuthorizeNotification(authorizeRequest);

            return new Response<long>(request.Id, $"Successfully authorized pin request with Id - {request.Id}");
        }

        public async Task<Response<ValidatePinUserResponse>> ValidatePinUser(ValidatePinUserRequest request)
        {
            // Query User to confirm user is registered for mobile banking
            UserIdRequest queryRequest = new()
            {
                user_id = request.UserId
            };
            QueryUserResponse queryResponse = await _apiCall.QueryUserByUserId(queryRequest);
            if (queryResponse == null)
            {
                throw new ApiException($"An error occured while querying user.");
            }
            else if (queryResponse.response_code == "04")
            {
                throw new ApiException($"User is not registered for mobile banking.");
            }
            else if (queryResponse.response_code != "00")
            {
                throw new ApiException($"An error occured while calling query user.");
            }

            // [issue] We need an API to validate the account number provided against either the User ID or the Customer Id

            UserDevice userDevice = await _context.UserDevice
                .Where(u => u.UserId.ToLower() == request.UserId.ToLower())
                .FirstOrDefaultAsync();

            if (userDevice == null)
            {
                throw new ApiException($"User is not profiled on . Kindly advice the user to create a profile.");
            }

            // Find the user to determine if the Pin should be reset or created
            FindUserRequest findUserRequest = new()
            {
                channel = _apiAuthorizationHeaders.EsbChannelCode,
                user_id = request.UserId
            };

            ValidatePinUserResponse response = _mapper.Map<ValidatePinUserResponse>(queryResponse.response_data);

            FindUserResponse findUserResponse = await _apiCall.FindUser(findUserRequest);

            if (findUserResponse?.response_code == "00")
            {
                response.PinUpdateType = PinUpdateType.ResetPin;
            }
            else if (findUserResponse?.response_code == "04")
            {
                response.PinUpdateType = PinUpdateType.CreatePin;
            }
            else
            {
                throw new ApiException($"An error occured while validating if the user has a pin entry.");
            }

            return new Response<ValidatePinUserResponse>(response, $"Successfully validated User with Id - {request.UserId}");
        }

        public PagedResponse<List<PendingPinResponse>> GetPendingPinRequests(PinQueryParameters queryParameters)
        {
            var username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;

            IQueryable<PendingPinManagement> pagedData = _context.PinManagement
                .Where(x => x.AuthStatus == AuthStatus.Pending && x.Authorizer == username)
                .Select(x => new PendingPinManagement
                {
                    Id = x.Id,
                    AccountNo = x.AccountNo,
                    UserId = x.UserId,
                    CustomerId = x.CustomerId,
                    Mobile = x.Mobile,
                    Email = x.Email,
                    UserStatus = x.UserStatus,
                    UpdateType = x.UpdateType,
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
                   || x.AccountNo.ToLower().Contains(query.ToLower())
                   || x.CustomerId.ToLower().Contains(query.ToLower())
                   || x.Initiator.ToLower().Contains(query.ToLower())
                   || x.Mobile.ToLower().Contains(query.ToLower()));
            }

            List<PendingPinManagement> pendingPinList = pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToList();

            List<PendingPinResponse> response = _mapper.Map<List<PendingPinManagement>, List<PendingPinResponse>>(pendingPinList);

            int totalRecords = pagedData.ToList().Count;

            return new PagedResponse<List<PendingPinResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved pending pin requests.");
        }

        public async Task<Response<PendingPinResponse>> GetPendingPinRequestById(long id)
        {
            PinManagement pinManagement = await _context.PinManagement.FindAsync(id);

            if (pinManagement == null)
            {
                throw new ApiException($"No pin request found for Pin ID - {id}.");
            }

            PendingPinResponse response = _mapper.Map<PinManagement, PendingPinResponse>(pinManagement);
            response.FileDocument = ConvertToBase64DataUrl(pinManagement.SupportingDocument, pinManagement.FileExtension);
            response.FileName = pinManagement.FileName;

            return new Response<PendingPinResponse>(response, $"Successfully retrieved pending pin request details with Id - {id}");
        }

        /**
         * Private Functions for internal operations
         **/
        private static string ConvertToBase64DataUrl(byte[] request, string extension)
        {
            if (extension == ".pdf" && request != null)
            {
                return $"data:application/{extension.Replace(".", "").Trim()};base64,{Convert.ToBase64String(request)}";
            }
            else if ((extension == ".jpg" || extension == ".png") && request != null)
            {
                return $"data:image/{extension.Replace(".", "").Trim()};base64,{Convert.ToBase64String(request)}";
            }
            else
            {
                return default;
            }
        }

        private async Task<BankApiResponse> CreatePin(PinManagement request)
        {
            CreatePinApiRequest createPinApiRequest = _mapper.Map<CreatePinApiRequest>(request);
            createPinApiRequest.reference = Utils.GenerateRandomCharacters(8);
            createPinApiRequest.pin = Utils.GenerateRandomCharacters(4);
            createPinApiRequest.channel = _apiAuthorizationHeaders.EsbChannelCode;

            return await _apiCall.CreatePin(createPinApiRequest);
        }

        private async Task<BankApiResponse> ResetPin(PinManagement request)
        {
            ResetPinApiRequest resetPinApiRequest = _mapper.Map<ResetPinApiRequest>(request);
            resetPinApiRequest.reference = $"AMP{Utils.GenerateRandomCharacters(8)}";
            resetPinApiRequest.channel = _apiAuthorizationHeaders.EsbChannelCode;

            return await _apiCall.ResetPin(resetPinApiRequest);
        }
    }
}
