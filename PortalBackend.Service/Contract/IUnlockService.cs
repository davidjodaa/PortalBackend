using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IUnlockService
    {
        Task<Response<long>> AddUnlockRequest(AddUnlockRequest request);
        Task<Response<long>> AuthorizeUnlockRequest(AuthRequest request);
        Task<Response<ValidateUnlockUserResponse>> ValidateUnlockUser(ValidateUnlockUserRequest request);
        PagedResponse<List<PendingUnlockResponse>> GetPendingUnlockRequests(UnlockQueryParameters queryParameters);
        Task<Response<PendingUnlockResponse>> GetPendingUnlockRequestById(long id);
    }
}
