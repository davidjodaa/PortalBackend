using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortalBackend.Domain.Auth;
using PortalBackend.Domain.Common;
using PortalBackend.Domain.QueryParameters;
using PortalBackend.Domain.Settings;
using PortalBackend.Service.Contract;
using PortalBackend.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PortalBackend.Persistence;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Net;
using AutoMapper;
using PortalBackend.Domain.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortalBackend.Service.Helpers;

namespace PortalBackend.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly JWTSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActiveDirectoryService _activeDirectory;
        private readonly IApplicationDbContext _context;
        private readonly IAPIImplementation _apiCall;
        private readonly IAppSessionService _appSession;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly ApiAuthorizationHeaders _apiAuthorizationHeaders;
        private readonly Secrets _secrets;
        private readonly INotificationService _notificationService;

        public AccountService(IOptions<JWTSettings> jwtSettings,
            IActiveDirectoryService activeDirectory,
            IApplicationDbContext context,
            IAPIImplementation apiCall,
            IHttpContextAccessor httpContextAccessor,
            IAppSessionService appSession,
            IMapper mapper,
            ILogger<AccountService> logger,
            IOptions<ApiAuthorizationHeaders> apiAuthorizationHeaders,
            IOptions<Secrets> secrets,
            INotificationService notificationService)
        {
            _jwtSettings = jwtSettings.Value;
            _activeDirectory = activeDirectory;
            _context = context;
            _apiCall = apiCall;
            _httpContextAccessor = httpContextAccessor;
            _appSession = appSession;
            _mapper = mapper;
            _logger = logger;
            _apiAuthorizationHeaders = apiAuthorizationHeaders.Value;
            _secrets = secrets.Value;
            _notificationService = notificationService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            // Check for the username
            User user = await _context.User
                            .Where(u => u.UserName == request.Username)
                            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Username}.");
            }

            if (!user.IsActive)
            {
                throw new ApiException($"This account has not been authorized for use.");
            }

            // Delete existing session if the user is already logged in
            _appSession.DeleteExistingSession(request.Username);

            // Verify the username and password
            bool isLoggedIn = _activeDirectory.Login(request.Username, request.Password);
            if (!isLoggedIn)
            {
                throw new ApiException($"Active Directory authentication failed - {request.Username}.");
            }

            // Verify the token if it is enabled in the settings
            if (_secrets.EnableToken)
            {
                bool isTokenValid = await _verifyToken(request);
                if (!isTokenValid)
                {
                    throw new ApiException($"Token validation failed - {request.Token}.");
                }
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = _mapper.Map<User, AuthenticationResponse>(user);

            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            response.ExpiresIn = _jwtSettings.DurationInMinutes * 60;
            response.ExpiryDate = DateTime.Now.AddSeconds((_jwtSettings.DurationInMinutes * 60));

            user.IsLoggedIn = true;
            user.LastLoginTime = DateTime.Now;

            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            DateTime utcNow = DateTime.UtcNow;
            string ipAddress = IpHelper.GetIpAddress();
            string sessionKey = Guid.NewGuid().ToString();
            await _appSession.CreateSession(sessionKey, user.UserName);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                new Claim("name", user?.Name ?? ""),
                new Claim("emailAddress", user?.Email ?? ""),
                new Claim("branchCode", user?.Branch ?? ""),
                new Claim("sessionId", sessionKey ?? ""),
                new Claim("username", user?.UserName ?? ""),
                new Claim("roleId", user?.RoleId ?? ""),
                new Claim("ip", user?.MobileNumber ?? ""),
                new Claim("ip", ipAddress ?? "")
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<Response<string>> LogoutAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            var sessionId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;

            // Short circuit if there is no username or password in the JWT token
            if (username == null && sessionId == null)
            {
                return new Response<string>($"The JWT token has expired", (int)HttpStatusCode.OK, true);
            }

            User user = await _context.User
                            .Where(u => u.UserName == username)
                            .FirstOrDefaultAsync();

            user.IsLoggedIn = false;
            _context.User.Update(user);

            _appSession.DeleteSession(sessionId, username);
            await _context.SaveChangesAsync();

            return new Response<string>($"Successfully logged out user with username - {username}", (int)HttpStatusCode.OK, true);
        }

        public async Task<PagedResponse<List<UserResponse>>> GetUsersAsync(UserQueryParameters queryParameters)
        {
            IQueryable<User> pagedData = _context.User
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();

            string query = queryParameters.Query;

            // Check if there is a query and apply it
            if (!string.IsNullOrEmpty(query))
            {
                pagedData = pagedData.Where(x => x.Id.ToString().Contains(query.ToLower())
                   || x.UserName.ToLower().Contains(query.ToLower())
                   || x.Email.ToLower().Contains(query.ToLower())
                   || x.MobileNumber.ToLower().Contains(query.ToLower()));
            }

            List<User> userList = await pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            List<UserResponse> response = _mapper.Map<List<User>, List<UserResponse>>(userList);

            int totalRecords = pagedData.ToList().Count;

            return new PagedResponse<List<UserResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved users");
        }

        public async Task<Response<UserResponse>> GetUserById(long id)
        {
            User userData = await _context.User
                               .Where(u => u.Id == id)
                               .FirstOrDefaultAsync();

            if (userData == null)
            {
                throw new ApiException($"No user found for User ID - {id}.");
            }

            UserResponse response = _mapper.Map<User, UserResponse>(userData);

            return new Response<UserResponse>(response, $"Successfully retrieved user details for user with Id - {id}");
        }

        public async Task<PagedResponse<List<PendingUserListResponse>>> GetPendingUserRequest(UserQueryParameters queryParameters)
        {
            IQueryable<PendingUserRequest> pagedData = _context.PendingUserRequest
                .Where(x => x.AuthStatus == AuthStatus.Pending)
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            string query = queryParameters.Query;

            // Check if there is a query and apply it
            if (!string.IsNullOrEmpty(query))
            {
                pagedData = pagedData.Where(x => x.RoleId.ToLower().Contains(query.ToLower())
                   || x.UserName.ToLower().Contains(query.ToLower())
                   || x.Initiator.ToLower().Contains(query.ToLower()));
            }

            List<PendingUserRequest> pendingUserList = await pagedData.Skip((queryParameters.PageIndex - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            List<PendingUserListResponse> response = _mapper.Map<List<PendingUserRequest>, List<PendingUserListResponse>>(pendingUserList);

            int totalRecords = pagedData.ToList().Count;

            return new PagedResponse<List<PendingUserListResponse>>(response, queryParameters.PageIndex, queryParameters.PageSize, totalRecords, $"Successfully retrieved users");
        }

        public async Task<Response<PendingUserResponse>> GetPendingUserRequestById(long id)
        {
            PendingUserRequest pendingUserRequest = await _context.PendingUserRequest
                .Where(u => u.Id == id && u.AuthStatus == AuthStatus.Pending)
                .FirstOrDefaultAsync();

            if (pendingUserRequest == null)
            {
                throw new ApiException($"No pending user request found for Pin ID - {id}.");
            }

            PendingUserResponse response = _mapper.Map<PendingUserRequest, PendingUserResponse>(pendingUserRequest);

            return new Response<PendingUserResponse>(response, $"Successfully retrieved pending user details for user with Id - {id}");
        }

        public async Task<Response<long>> AddUserAsync(UserRequest request)
        {
            User userWithSameUserName = await _context.User
                .Where(u => u.UserName == request.UserName)
                .FirstOrDefaultAsync();

            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already registered.");
            }

            Roles roles = await _context.Roles
                .Where(r => r.RoleId == request.Role)
                .FirstOrDefaultAsync();

            // Check if the role exists
            if (roles == null)
            {
                throw new ApiException($"This role doesn't exists. Please check your role and try again");
            }

            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            PendingUserRequest pendingUserRequest = _mapper.Map<PendingUserRequest>(request);
            pendingUserRequest.RequestType = UserRequestType.New;
            pendingUserRequest.Initiator = username;
            pendingUserRequest.InitiatorEmail = email;
            pendingUserRequest.InitiatingBranch = branch;
            pendingUserRequest.DateInitiated = DateTime.Now;


            // Get all the pending requests that haven't been treated
            List<PendingUserRequest> untreatedPendingReq = await _context.PendingUserRequest
                .Where(pr => pr.AuthStatus == AuthStatus.Pending && pr.UserName == request.UserName)
                .ToListAsync();

            foreach (var item in untreatedPendingReq)
            {
                item.AuthStatus = AuthStatus.Reinitiated;
                item.AuthorizersComment = "This request was reinitiated";
            }

            _context.PendingUserRequest.UpdateRange(untreatedPendingReq);
            await _context.PendingUserRequest.AddAsync(pendingUserRequest);
            await _context.SaveChangesAsync();

            return new Response<long>(pendingUserRequest.Id, message: $"Successfully registered user with username - {request.UserName}");
        }

        public async Task<Response<long>> EditUserAsync(EditUserRequest request)
        {
            User user = await _context.User
                .Where(u => u.UserName == request.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ApiException($"Username '{request.UserName}' could not be found.");
            }

            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            PendingUserRequest pendingUserRequest = new PendingUserRequest()
            {
                UserName = request.UserName,
                Name = string.IsNullOrEmpty(request.Name) ? user.Name : request.Name,
                Email = string.IsNullOrEmpty(request.EmailAddress) ? user.Email : request.EmailAddress,
                MobileNumber = string.IsNullOrEmpty(request.MobileNo) ? user.MobileNumber : request.MobileNo,
                Group = string.IsNullOrEmpty(request.StaffGroup) ? user.Group : request.StaffGroup,
                Branch = string.IsNullOrEmpty(request.StaffBranch) ? user.Branch : request.StaffBranch,
                RoleId = string.IsNullOrEmpty(request.Role) ? user.RoleId : request.Role,
                RequestType = UserRequestType.Edit,
                Initiator = username,
                InitiatorEmail = email,
                InitiatingBranch = branch,
                DateInitiated = DateTime.Now
            };

            // Get all the pending requests that haven't been treated
            List<PendingUserRequest> untreatedPendingReq = await _context.PendingUserRequest
                .Where(pr => pr.AuthStatus == AuthStatus.Pending && pr.UserName == request.UserName)
                .ToListAsync();

            foreach (var item in untreatedPendingReq)
            {
                item.AuthStatus = AuthStatus.Reinitiated;
                item.AuthorizersComment = "This request was reinitiated";
            }

            _context.PendingUserRequest.UpdateRange(untreatedPendingReq);
            await _context.PendingUserRequest.AddAsync(pendingUserRequest);
            await _context.SaveChangesAsync();

            return new Response<long>(pendingUserRequest.Id, message: $"Successfully edited user with username - {request.UserName}");
        }

        public async Task<Response<long>> DeleteUserAsync(DeleteUserRequest request)
        {
            User user = await _context.User
                            .Where(u => u.UserName == request.UserName)
                            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ApiException($"Username '{request.UserName}' could not be found.");
            }

            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            PendingUserRequest pendingUserRequest = _mapper.Map<PendingUserRequest>(user);
            pendingUserRequest.RequestType = UserRequestType.Delete;
            pendingUserRequest.Initiator = username;
            pendingUserRequest.InitiatorEmail = email;
            pendingUserRequest.InitiatingBranch = branch;
            pendingUserRequest.DateInitiated = DateTime.Now;

            // Get all the pending requests that haven't been treated
            List<PendingUserRequest> untreatedPendingReq = await _context.PendingUserRequest
                .Where(pr => pr.AuthStatus == AuthStatus.Pending && pr.UserName == request.UserName)
                .ToListAsync();

            foreach (var item in untreatedPendingReq)
            {
                item.AuthStatus = AuthStatus.Reinitiated;
                item.AuthorizersComment = "This request was reinitiated";
            }

            _context.PendingUserRequest.UpdateRange(untreatedPendingReq);
            await _context.PendingUserRequest.AddAsync(pendingUserRequest);
            await _context.SaveChangesAsync();

            return new Response<long>(pendingUserRequest.Id, message: $"Successfully deleted user with username - {request.UserName}");
        }

        public async Task<Response<long>> AuthorizeUserAsync(StringAuthRequest request)
        {
            PendingUserRequest pendingUserRequest = await _context.PendingUserRequest
                .Where(u => u.Id == request.Id && u.AuthStatus == AuthStatus.Pending)
                .FirstOrDefaultAsync();

            if (pendingUserRequest == null)
            {
                throw new ApiException($"No pending user found with Id - {request.Id}.");
            }

            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            string email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value;
            string branch = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "branchCode")?.Value;

            if (username == pendingUserRequest.Initiator && _secrets.EnableSelfAuthCheck)
            {
                throw new ApiException($"Self authorization is not allowed.");
            }

            if (request.AuthStatus == AuthStatus.Approved)
            {
                switch (pendingUserRequest.RequestType)
                {
                    case UserRequestType.New:
                        await _AddUser(pendingUserRequest);
                        break;
                    case UserRequestType.Edit:
                        await _EditUser(pendingUserRequest);
                        break;
                    case UserRequestType.Delete:
                        await _DeleteUser(pendingUserRequest);
                        break;
                    default:
                        throw new ApiException($"Invalid request type by the initiator.");
                }
            }

            pendingUserRequest.Authorizer = username;
            pendingUserRequest.AuthorizerEmail = email;
            pendingUserRequest.DateAuthorized = DateTime.Now;
            pendingUserRequest.AuthStatus = request.AuthStatus;
            pendingUserRequest.AuthorizersComment = request.AuthorizersComment;

            _context.PendingUserRequest.Update(pendingUserRequest);
            await _context.SaveChangesAsync();

            return new Response<long>(pendingUserRequest.Id, message: $"Successfully authorized user with Id - {request.Id}");
        }

        public async Task<Response<ValidateUserResponse>> ValidateUserAsync(string username)
        {
            User userWithSameUserName = await _context.User
                            .Where(u => u.UserName.ToLower() == username.ToLower())
                            .FirstOrDefaultAsync();

            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{username}' is already registered.");
            }

            UserDetailsResponse adUser = _activeDirectory.GetUserDetails(username);

            ValidateUserResponse response = _mapper.Map<UserDetailsResponse, ValidateUserResponse>(adUser);

            return new Response<ValidateUserResponse>(response, $"Successfully valideated user with username - {username}");
        }

        public async Task<AuthorizerResponse> GetUserInRole(string userName, string role)
        {
            User userData = await _context.User
                .Where(u => u.UserName == userName)
                .FirstOrDefaultAsync();

            if (userData == null)
            {
                _logger.LogError($"No user found with username - {userName}.");
                return null;
            }

            if (userData.RoleId != role)
            {
                _logger.LogError($"User with username {userName} is not an {role}.");
                return null;
            }

            AuthorizerResponse response = _mapper.Map<User, AuthorizerResponse>(userData);

            return response;
        }

        /**
         * Private Functions for internal operations
         **/
        private async Task<User> _AddUser(PendingUserRequest request)
        {
            string username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;

            User newUser = _mapper.Map<User>(request);
            newUser.IsActive = true;

            await _context.User.AddAsync(newUser);

            WelcomeNotificationRequest welcomeRequest = _mapper.Map<WelcomeNotificationRequest>(request);
            await _notificationService.WelcomeNotification(welcomeRequest);

            return newUser;
        }

        private async Task<User> _EditUser(PendingUserRequest request)
        {
            User user = await _context.User
                            .Where(u => u.UserName == request.UserName)
                            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ApiException($"No user found with username '{request.UserName}'.");
            }

            user.Branch = request.Branch;
            user.Email = request.Email;
            user.Name = request.Name;
            user.Group = request.Group;
            user.MobileNumber = request.MobileNumber;
            user.RoleId = request.RoleId;

            _context.User.Update(user);

            return user;
        }

        private async Task<User> _DeleteUser(PendingUserRequest request)
        {
            User user = await _context.User
                            .Where(u => u.UserName == request.UserName)
                            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ApiException($"The user with User Name - {request.UserName} does not exist.");
            }

            _context.User.Remove(user);

            return user;
        }

        private async Task<bool> _verifyToken(AuthenticationRequest request)
        {
            Verify2FARequest verify2FARequest = new()
            {
                UserName = request.Username,
                GroupName = _apiAuthorizationHeaders.TokenGroupName,
                Password = request.Token
            };

            Verify2FAResponse response = await _apiCall.Verify2FAToken(verify2FARequest);

            return response?.response_code == "00";
        }
    }
}
