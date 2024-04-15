using PortalBackend.Service.DTO;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IAPIImplementation
    {
        Task<AcctDetailsResponse> GetAccounttDetails(AcctDetailsRequest request);
        Task<ActiveDirectoryDetailsResponse> GetActiveDirectoryDetails(ActiveDirectoryDetailsRequest request);
        Task<SendMailResponse> SendMail(SendMailRequest request);
        Task<AuthUserResponse> ActiveDirectoryLogin(AuthUserRequest request, string authorizationHeader);
        Task<BankApiResponse> ResetPin(ResetPinApiRequest request);
        Task<BankApiResponse> CreatePin(CreatePinApiRequest request);
        Task<QueryUserResponse> QueryUserByUserId(UserIdRequest request);
        Task<FindUserResponse> FindUser(FindUserRequest request);
        Task<Verify2FAResponse> Verify2FAToken(Verify2FARequest request);
        Task<BankApiResponse> UpdateUser(UpdateUserRequest request);
        Task<UserApiResponse> EnableUser(UserIdRequest request);
        Task<UserApiResponse> DisableUser(UserIdRequest request);
    }
}
