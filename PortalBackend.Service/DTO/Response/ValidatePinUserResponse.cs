
using PortalBackend.Domain.Enum;

namespace PortalBackend.Service.DTO.Response
{
    public class ValidatePinUserResponse
    {
        public PinUpdateType PinUpdateType { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string UserRole { get; set; }
        public string UserStatus { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
