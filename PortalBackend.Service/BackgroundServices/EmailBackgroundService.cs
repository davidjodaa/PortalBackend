using System;
using System.Threading;
using System.Threading.Tasks;
using PortalBackend.Service.Contract;
using PortalBackend.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PortalBackend.Service.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly ILogger<EmailBackgroundService> _logger;
        private readonly IQueueManager _queue;
        private readonly IServiceProvider _service;

        public EmailBackgroundService(ILogger<EmailBackgroundService> logger,
          IQueueManager queue,
          IServiceProvider service)
        {
            _logger = logger;
            _queue = queue;
            _service = service;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var emailRequest = await _queue.PullEmailAsync(stoppingToken);

                    _logger.LogInformation($"Processing email service for {emailRequest.Subject} to {emailRequest.Recipient}");

                    using var scope = _service.CreateScope();
                    var repo = scope.ServiceProvider.GetService<IAPIImplementation>();
                    await repo.SendMail(emailRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failure while processing queue {ex}");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Background Service");
            await base.StopAsync(cancellationToken);
        }
    }
}