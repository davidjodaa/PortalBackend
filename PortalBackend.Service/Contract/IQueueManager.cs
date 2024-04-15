using PortalBackend.Service.DTO.Request;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IQueueManager
    {
        ValueTask<SendMailRequest> PullEmailAsync(CancellationToken cancellationToken);
        ValueTask PushEmailAsync([NotNull] SendMailRequest request);
    }
}
