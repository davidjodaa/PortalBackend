using PortalBackend.Domain.Enum;
using System;

namespace PortalBackend.Service.DTO.Response
{
    public class PendingPinResponse
    {
        public long Id { get; set; }
        public string AccountNo { get; set; }
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string UserStatus { get; set; }
        public string UserId { get; set; }
        public PinUpdateType UpdateType { get; set; }
        public string FileName { get; set; }
        public string FileDocument { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Initiator { get; set; }
        public string InitiatorEmail { get; set; }
        public DateTime DateInitiated { get; set; }
        public string InitiatingBranch { get; set; }
    }
}
