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
    public class PinController : Controller
    {
        private readonly IPinService _PinService;
        private readonly IValidator<AuthRequest> _authRequestValidator;
        private readonly IValidator<AddPinRequest> _addPinRequestValidator;
        public PinController(IPinService PinService,
            IValidator<AuthRequest> authRequestValidator,
            IValidator<AddPinRequest> addPinRequestValidator)
        {
            _PinService = PinService;
            _authRequestValidator = authRequestValidator;
            _addPinRequestValidator = addPinRequestValidator;
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("addPin")]
        public async Task<IActionResult> AddPin([FromForm] AddPinRequest request)
        {
            //await _addPinRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _PinService.AddPinRequest(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpPost("authorizePin")]
        public async Task<IActionResult> AuthorizePin([FromBody] AuthRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _PinService.AuthorizePinRequest(request));
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("validatePinUser")]
        public async Task<IActionResult> ValidatePinUser([FromBody] ValidatePinUserRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _PinService.ValidatePinUser(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingPinRequests")]
        public IActionResult GetPendingPinRequests([FromQuery] PinQueryParameters queryParameters)
        {
            return Ok(_PinService.GetPendingPinRequests(queryParameters));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingPinRequest/{id}")]
        public async Task<IActionResult> GetPendingPinRequestById(long id)
        {
            return Ok(await _PinService.GetPendingPinRequestById(id));
        }
    }
}
