using PortalBackend.Domain.Auth;
using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IAccountService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<Response<string>> LogoutAsync();
        Task<Response<long>> AddUserAsync(UserRequest request);
        Task<Response<long>> EditUserAsync(EditUserRequest request);
        Task<Response<long>> DeleteUserAsync(DeleteUserRequest request);
        Task<Response<long>> AuthorizeUserAsync(StringAuthRequest request);
        Task<Response<ValidateUserResponse>> ValidateUserAsync(string username);
        Task<PagedResponse<List<UserResponse>>> GetUsersAsync(UserQueryParameters queryParameters);
        Task<Response<UserResponse>> GetUserById(long id);
        Task<PagedResponse<List<PendingUserListResponse>>> GetPendingUserRequest(UserQueryParameters queryParameters);
        Task<Response<PendingUserResponse>> GetPendingUserRequestById(long id);
        Task<AuthorizerResponse> GetUserInRole(string userName, string role);
    }
}
