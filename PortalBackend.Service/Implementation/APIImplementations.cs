using PortalBackend.Domain.Settings;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class APIImplementations : IAPIImplementation
    {
        private readonly IClientFactory _clientFactory;
        private readonly ILogger<APIImplementations> _logger;
        private readonly ApiResourceUrls _apiResourceUrls;
        private readonly ApiAuthorizationHeaders _apiAuthorizationHeaders;

        public APIImplementations(IClientFactory clientFactory,
            ILogger<APIImplementations> logger,
            IOptions<ApiResourceUrls> apiResourceUrls,
            IOptions<ApiAuthorizationHeaders> apiAuthorizationHeaders)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _apiResourceUrls = apiResourceUrls.Value;
            _apiAuthorizationHeaders = apiAuthorizationHeaders.Value;
        }

        public async Task<AcctDetailsResponse> GetAccounttDetails(AcctDetailsRequest request)
        {
            AcctDetailsResponse response = await _clientFactory.PostDataWithCsrfAsync<AcctDetailsResponse, AcctDetailsRequest>(_apiResourceUrls.MaintenanceEnquiry, _apiResourceUrls.MaintenanceEnquiryToken, request);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling {nameof(GetActiveDirectoryDetails)}.");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<ActiveDirectoryDetailsResponse> GetActiveDirectoryDetails(ActiveDirectoryDetailsRequest request)
        {
            ActiveDirectoryDetailsResponse response = await _clientFactory.PostDataAsync<ActiveDirectoryDetailsResponse, ActiveDirectoryDetailsRequest>(_apiResourceUrls.GetActiveDirectoryDetails, request);

            if (response?.ResponseCode != "00")
            {
                _logger.LogError($"An error occured while calling {nameof(GetActiveDirectoryDetails)}.");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<SendMailResponse> SendMail(SendMailRequest request)
        {
            SendMailResponse response = await _clientFactory.PostDataAsync<SendMailResponse, SendMailRequest>(_apiResourceUrls.SendEmail, request, _apiAuthorizationHeaders.AuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling {nameof(SendMail)}.");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<AuthUserResponse> ActiveDirectoryLogin(AuthUserRequest request, string authorizationHeader)
        {
            AuthUserResponse response = await _clientFactory.PostDataAsync<AuthUserResponse, AuthUserRequest>(_apiResourceUrls.ActiveDirectoryLogin, request, authorizationHeader, true);

            if (response?.status != "00")
            {
                _logger.LogError($"An error occured while calling {nameof(ActiveDirectoryLogin)} for {request.username}.");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<BankApiResponse> ResetPin(ResetPinApiRequest request)
        {
            BankApiResponse response = await _clientFactory.PostDataAsync<BankApiResponse, ResetPinApiRequest>(_apiResourceUrls.ResetPin, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Reset Pin service for {request.user_id} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        // Todo: Link with Bosun to understand how this API works
        public async Task<BankApiResponse> UpdateUser(UpdateUserRequest request)
        {
            BankApiResponse response = await _clientFactory.PostDataAsync<BankApiResponse, UpdateUserRequest>(_apiResourceUrls.UpdateUser, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Reset Pin service for {request.userId} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<BankApiResponse> CreatePin(CreatePinApiRequest request)
        {
            BankApiResponse response = await _clientFactory.PostDataAsync<BankApiResponse, CreatePinApiRequest>(_apiResourceUrls.CreatePin, request, _apiAuthorizationHeaders.EsbAuthKey, true);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Create Pin service for {request.account_no} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<QueryUserResponse> QueryUserByUserId(UserIdRequest request)
        {
            QueryUserResponse response = await _clientFactory.PostDataAsync<QueryUserResponse, UserIdRequest>(_apiResourceUrls.QueryByUserId, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Query User service for {request.user_id} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<UserApiResponse> EnableUser(UserIdRequest request)
        {
            UserApiResponse response = await _clientFactory.PostDataAsync<UserApiResponse, UserIdRequest>(_apiResourceUrls.EnableUser, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Enable User service for {request.user_id} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<UserApiResponse> DisableUser(UserIdRequest request)
        {
            UserApiResponse response = await _clientFactory.PostDataAsync<UserApiResponse, UserIdRequest>(_apiResourceUrls.DisableUser, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Disable User service for {request.user_id} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<FindUserResponse> FindUser(FindUserRequest request)
        {
            FindUserResponse response = await _clientFactory.PostDataAsync<FindUserResponse, FindUserRequest>(_apiResourceUrls.FindUser, request, _apiAuthorizationHeaders.EsbAuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling Query User service for {request.user_id} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        public async Task<Verify2FAResponse> Verify2FAToken(Verify2FARequest request)
        {
            Verify2FAResponse response = await _clientFactory.PostDataAsync<Verify2FAResponse, Verify2FARequest>(_apiResourceUrls.Verify2FA, request, _apiAuthorizationHeaders.AuthKey);

            if (response?.response_code != "00")
            {
                _logger.LogError($"An error occured while calling verify token service for {request.UserName} with error message {response?.response_message ?? "null response"}");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }

        private async Task<FetchTokenResponse> FetchToken(string requestUrl)
        {
            FetchTokenResponse response = await _clientFactory.GetDataAsync<FetchTokenResponse>(requestUrl);

            if (response?.token != "X-CSRF-Token")
            {
                _logger.LogError($"An error occured while fetching token for {requestUrl} with the response below");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }
    }
}
