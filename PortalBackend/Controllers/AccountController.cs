using Microsoft.AspNetCore.Mvc;
using PortalBackend.Domain.Auth;
using PortalBackend.Service.Contract;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Domain.QueryParameters;
using FluentValidation;
using PortalBackend.Service.CustomAttributes;

namespace PortalBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _accountService.AuthenticateAsync(request));
        }
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await _accountService.LogoutAsync());
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParameters queryParameters)
        {
            return Ok(await _accountService.GetUsersAsync(queryParameters));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            return Ok(await _accountService.GetUserById(id));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(UserRequest request)
        {
            return Ok(await _accountService.AddUserAsync(request));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpPost("editUser")]
        public async Task<IActionResult> EditUser(EditUserRequest request)
        {
            return Ok(await _accountService.EditUserAsync(request));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpPost("deleteUser")]
        public async Task<IActionResult> DeleteUser(DeleteUserRequest request)
        {
            return Ok(await _accountService.DeleteUserAsync(request));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpGet("validateUser")]
        public async Task<IActionResult> ValidateUser([FromQuery] string userId)
        {
            return Ok(await _accountService.ValidateUserAsync(userId));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpPost("authorizeUser")]
        public async Task<IActionResult> AuthorizeUser([FromBody] StringAuthRequest request)
        {
            return Ok(await _accountService.AuthorizeUserAsync(request));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpGet("pendingUserRequest")]
        public async Task<IActionResult> GetPendingUserRequest([FromQuery] UserQueryParameters queryParameters)
        {
            return Ok(await _accountService.GetPendingUserRequest(queryParameters));
        }
        [MiddlewareFilter(typeof(SessionAttributeFilter))]
        [HttpGet("getPendingUser/{id}")]
        public async Task<IActionResult> GetPendingUserById(long id)
        {
            return Ok(await _accountService.GetPendingUserRequestById(id));
        }
    }
}