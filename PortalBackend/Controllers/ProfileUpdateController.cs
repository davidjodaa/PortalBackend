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
    public class ProfileUpdateController : Controller
    {
        private readonly IProfileUpdateService _ProfileService;
        //private readonly IValidator<AuthRequest> _authRequestValidator;
        //private readonly IValidator<AddProfileRequest> _addProfileRequestValidator;
        public ProfileUpdateController(IProfileUpdateService ProfileService)
        {
            _ProfileService = ProfileService;
            //_authRequestValidator = authRequestValidator;
            //_addProfileRequestValidator = addProfileRequestValidator;
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("addProfileUpdate")]
        public async Task<IActionResult> AddProfileRequest([FromForm] AddProfileRequest request)
        {
            //await _addProfileRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _ProfileService.AddProfileRequest(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpPost("authorizeProfileUpdate")]
        public async Task<IActionResult> AuthorizeProfileRequest([FromBody] AuthRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _ProfileService.AuthorizeProfileRequest(request));
        }

        [Authorize(Roles = "Administrator,Initiator")]
        [HttpPost("validateProfileUpdateUser")]
        public async Task<IActionResult> ValidateProfileUser([FromBody] ValidateProfileUserRequest request)
        {
            //await _authRequestValidator.ValidateAndThrowAsync(request);
            return Ok(await _ProfileService.ValidateProfileUser(request));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingProfileUpdateRequests")]
        public IActionResult GetPendingProfileUpdateRequests([FromQuery] ProfileQueryParameters queryParameters)
        {
            return Ok(_ProfileService.GetPendingProfileRequests(queryParameters));
        }

        [Authorize(Roles = "Administrator,Authorizer")]
        [HttpGet("pendingProfileUpdateRequest/{id}")]
        public async Task<IActionResult> GetPendingProfileUpdateRequestById(long id)
        {
            return Ok(await _ProfileService.GetPendingProfileRequestById(id));
        }
    }
}
