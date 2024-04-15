using PortalBackend.Domain.QueryParameters;
using PortalBackend.Service.Contract;
using PortalBackend.Service.CustomAttributes;
using PortalBackend.Service.DTO.Request;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PortalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(SessionAttributeFilter))]
    public class DeviceUnlockController : Controller
    {
        private readonly IUnlockService _UnlockService;
        public DeviceUnlockController(IUnlockService UnlockService)
        {
            _UnlockService = UnlockService;
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("addDeviceUnlock")]
        public async Task<IActionResult> AddUnlockRequest([FromForm] AddUnlockRequest request)
        {
            //await _addUnlockRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _UnlockService.AddUnlockRequest(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpPost("authorizeDeviceUnlock")]
        public async Task<IActionResult> AuthorizeUnlockRequest([FromBody] AuthRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _UnlockService.AuthorizeUnlockRequest(request));
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("validateUnlockUser")]
        public async Task<IActionResult> ValidateUnlockUser([FromBody] ValidateUnlockUserRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _UnlockService.ValidateUnlockUser(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingUnlockRequests")]
        public IActionResult GetPendingUnlockRequests([FromQuery] UnlockQueryParameters queryParameters)
        {
            return Ok(_UnlockService.GetPendingUnlockRequests(queryParameters));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingUnlockRequest/{id}")]
        public async Task<IActionResult> GetPendingUnlockRequestById(long id)
        {
            return Ok(await _UnlockService.GetPendingUnlockRequestById(id));
        }
    }
}
