using PortalBackend.Domain.Entities;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface INotificationService
    {
        Task InitiateRequestNotification(InitiateRequestNotification request);
        Task AuthorizeNotification(AuthorizeRequestNotification request);
        Task WelcomeNotification(WelcomeNotificationRequest request);
    }
}
