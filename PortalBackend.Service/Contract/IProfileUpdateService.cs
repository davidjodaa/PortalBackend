using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IProfileUpdateService
    {
        Task<Response<long>> AddProfileRequest(AddProfileRequest request);
        Task<Response<long>> AuthorizeProfileRequest(AuthRequest request);
        Task<Response<ValidateProfileUserResponse>> ValidateProfileUser(ValidateProfileUserRequest request);
        PagedResponse<List<PendingProfileResponse>> GetPendingProfileRequests(ProfileQueryParameters queryParameters);
        Task<Response<PendingProfileResponse>> GetPendingProfileRequestById(long id);
    }
}
