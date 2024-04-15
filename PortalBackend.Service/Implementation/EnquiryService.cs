using PortalBackend.Domain.Auth;
using PortalBackend.Domain.Common;
using PortalBackend.Domain.Entities;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class EnquiryService : IEnquiryService
    {
        private readonly IAPIImplementation _apiCall;
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EnquiryService> _logger;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly ApiAuthorizationHeaders _apiAuthorizationHeaders;

        public EnquiryService(IAPIImplementation apiCall,
            IApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EnquiryService> logger,
            IAccountService accountService,
            IMapper mapper,
            IOptions<ApiAuthorizationHeaders> apiAuthorizationHeaders)
        {
            _apiCall = apiCall;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _accountService = accountService;
            _mapper = mapper;
            _apiAuthorizationHeaders = apiAuthorizationHeaders.Value;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * Todo :: Rewrite this funtion to reduce the roundtripping to the database
        **/
        public async Task<Response<GetUserEnquiryDetailsResponse>> GetUserEnquiryDetails(GetUserEnquiryDetailsRequest request)
        {
            QueryUserResponse queryUserResponse = new QueryUserResponse();
            // if User Id is not passed then account number should be passed as long as our model validation is working correctly
            if (string.IsNullOrEmpty(request.UserId))
            {
                AcctDetailsResponse customerInfo = await GetCustomerInfoByAccountNo(request.AccountNo);

                string customerId = customerInfo?.getcustomeracctsdetailsresp?.FirstOrDefault()?.custID ?? null;

                if (customerId == null)
                {
                    throw new ApiException($"No customer information found for account number - {request.AccountNo}.");
                }

                request.UserId = await _context.UserDevice
                    .Where(u => u.CustomerId == customerId)
                    .Select(u => u.UserId)
                    .FirstOrDefaultAsync();

                if (request.UserId == null)
                {
                    throw new ApiException($"No user profiled on  with the Customer Id {customerId}.");
                }
            }

            UserIdRequest queryByUserIdRequest = new UserIdRequest()
            {
                user_id = request.UserId
            };
            queryUserResponse = await _apiCall.QueryUserByUserId(queryByUserIdRequest);

            if (queryUserResponse?.response_code != "00")
            {
                throw new ApiException($"No user details found.");
            }

            GetUserEnquiryDetailsResponse response = _mapper.Map<QueryUserResponseData, GetUserEnquiryDetailsResponse>(queryUserResponse.response_data);

            // Get the device details from the database
            UserDevice deviceStatus = await _context.UserDevice
                .Where(d => d.UserId == queryUserResponse.response_data.user_id.ToLower())
                .FirstOrDefaultAsync();
            
            // Check if the user has a device registered on  and if he does then further check for the status of that device
            if (deviceStatus != null)
            {
                response.ProfileStatus = deviceStatus.DeviceStatus == "1" ? "ENABLED" : "DISABLED";
                response.AppVersion = deviceStatus.AppVersion;
                response.DeviceType = deviceStatus.Uuid?.Length > 20 ? "IOS" : "Android";
                response.RegistrationDate = deviceStatus.CreatedDate;
                response.LastLoginDate = deviceStatus.LastLoginDay;
            }

            return new Response<GetUserEnquiryDetailsResponse>(response, $"Successfully retrieved enquiry details for the user");
        }

        public async Task<AcctDetailsResponse> GetCustomerInfoByAccountNo(string accountNo)
        {
            AcctDetailsRequest getCustomerInfoRequest = new AcctDetailsRequest()
            {
                accountNumber = accountNo,
                channel_code = _apiAuthorizationHeaders.BankApiChannelCode
            };
            return await _apiCall.GetAccounttDetails(getCustomerInfoRequest);
        }
    }
}