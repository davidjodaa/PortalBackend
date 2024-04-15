using PortalBackend.Domain.Auth;
using PortalBackend.Service.DTO.Response;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IActiveDirectoryService
    {
        Task<AuthUserResponse> AuthenticateUser(string userId, string password);
        bool Login(string userId, string password);
        UserDetailsResponse GetUserDetails(string userName);
    }
}
