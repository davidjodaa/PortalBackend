using PortalBackend.Domain.Common;
using PortalBackend.Service.DTO.Response;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IUtilityService
    {
        Task<Response<AuthorizerResponse>> GetAuthorizerByUsername(string userName);
    }
}
