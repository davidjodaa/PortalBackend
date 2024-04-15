using PortalBackend.Domain.Common;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IEnquiryService
    {
        Task<Response<GetUserEnquiryDetailsResponse>> GetUserEnquiryDetails(GetUserEnquiryDetailsRequest request);
    }
}
