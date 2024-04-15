using PortalBackend.Domain.Entities;
using PortalBackend.Domain.Settings;
using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IAPIImplementation _apiCall;
        private readonly IQueueManager _queueManager;
        private readonly ILogger<NotificationService> _logger;
        private readonly ITemplateService _templateService;
        private readonly ApiAuthorizationHeaders _apiAuthorizationHeaders;

        public NotificationService(IAPIImplementation apiCall,
            IQueueManager queueManager,
            ILogger<NotificationService> logger,
            ITemplateService templateService,
            IOptions<ApiAuthorizationHeaders> apiAuthorizationHeaders)
        {
            _apiCall = apiCall;
            _queueManager = queueManager;
            _logger = logger;
            _templateService = templateService;
            _apiAuthorizationHeaders = apiAuthorizationHeaders.Value;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * 
        **/

        public async Task InitiateRequestNotification(InitiateRequestNotification request)
        {
            var emailRequest = new SendMailRequest()
            {
                Recipient = request.AuthorizersEmail,
                Subject = $"Pending {request.Action} from {request.Initiator}",
                Content = _templateService.GenerateHtmlStringFromViewsAsync("RequestNotification", request),
                DisplayName = " Portal"
            };

            await _queueManager.PushEmailAsync(emailRequest);
        }

        public async Task AuthorizeNotification(AuthorizeRequestNotification request)
        {
            var emailRequest = new SendMailRequest()
            {
                Recipient = request.InitiatorsEmail,
                Subject = $"{request.Action} Authorization :: {request.UserId}",
                Content = _templateService.GenerateHtmlStringFromViewsAsync("AuthorizeNotification", request),
                DisplayName = " Portal"
            };

            await _queueManager.PushEmailAsync(emailRequest);
        }

        public async Task WelcomeNotification(WelcomeNotificationRequest request)
        {
            var emailRequest = new SendMailRequest()
            {
                Recipient = request.Email,
                Subject = $"",
                Content = _templateService.GenerateHtmlStringFromViewsAsync("WelcomeNotification", request),
                DisplayName = ""
            };

            await _queueManager.PushEmailAsync(emailRequest);
        }
    }
}