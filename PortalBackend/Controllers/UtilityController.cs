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
    public class UtilityController : Controller
    {
        private readonly IUtilityService _UtilityService;
        public UtilityController(IUtilityService UtilityService)
        {
            _UtilityService = UtilityService;
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("getAuthorizerByUsername")]
        public async Task<IActionResult> GetAuthorizerByUsername(UsernameRequest request)
        {
            return Ok(await _UtilityService.GetAuthorizerByUsername(request.Username));
        }
    }
}
