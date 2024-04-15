using PortalBackend.Domain.Auth;
using PortalBackend.Domain.Common;
using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Entities;
using PortalBackend.Persistence;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using PortalBackend.Service.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class UtilityService : IUtilityService
    {
        private readonly IAccountService _accountService;

        public UtilityService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * 
        **/
        public async Task<Response<AuthorizerResponse>> GetAuthorizerByUsername(string userName)
        {
            AuthorizerResponse authorizer = await _accountService.GetUserInRole(userName, RoleConstants.Authorizer);

            if (authorizer == null)
            {
                throw new ApiException($"No authorizer found with username - {userName}.");
            }

            return new Response<AuthorizerResponse>(authorizer, $"Successfully retrieved authorizer with username - {userName}");
        }
    }
}