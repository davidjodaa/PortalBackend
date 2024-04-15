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
    public interface IPinService
    {
        Task<Response<long>> AddPinRequest(AddPinRequest request);
        Task<Response<long>> AuthorizePinRequest(AuthRequest request);
        Task<Response<ValidatePinUserResponse>> ValidatePinUser(ValidatePinUserRequest request);
        PagedResponse<List<PendingPinResponse>> GetPendingPinRequests(PinQueryParameters queryParameters);
        Task<Response<PendingPinResponse>> GetPendingPinRequestById(long id);
    }
}
