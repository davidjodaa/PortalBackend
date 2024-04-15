using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.Contract;
using PortalBackend.Service.CustomAttributes;
using PortalBackend.Service.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PortalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(SessionAttributeFilter))]
    public class EnquiryController : Controller
    {
        private readonly IEnquiryService _EnquiryService;
        public EnquiryController(IEnquiryService EnquiryService)
        {
            _EnquiryService = EnquiryService;
        }

        [Authorize(Roles = "Administrator,Initiator,Authorizer")]
        [HttpPost("getUserEnquiryDetails")]
        public async Task<IActionResult> GetUserEnquiryDetails(GetUserEnquiryDetailsRequest request)
        {
            return Ok(await _EnquiryService.GetUserEnquiryDetails(request));
        }
    }
}
