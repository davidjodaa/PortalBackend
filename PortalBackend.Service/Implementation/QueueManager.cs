using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class QueueManager : IQueueManager
    {
        private readonly Channel<SendMailRequest> _mailQueue;
        private readonly ILogger<QueueManager> _logger;

        public QueueManager(ILogger<QueueManager> logger)
        {
            var opts = new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.Wait };
            _mailQueue = Channel.CreateBounded<SendMailRequest>(opts);
            _logger = logger;
        }

        public async ValueTask PushEmailAsync([NotNull] SendMailRequest redirect)
        {
            await _mailQueue.Writer.WriteAsync(redirect);
            _logger.LogInformation("Added Email Request to Queue");
        }

        public async ValueTask<SendMailRequest> PullEmailAsync(CancellationToken cancellationToken)
        {
            var result = await _mailQueue.Reader.ReadAsync(cancellationToken);
            _logger.LogInformation("Removed Email Request from Queue");
            return result;
        }
    }
}
