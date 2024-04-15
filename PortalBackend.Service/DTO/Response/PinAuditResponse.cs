using PortalBackend.Domain;
using PortalBackend.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Service.DTO.Response
{
    public class PinAuditResponse : AuthorizableEntity
    {
        public string AccountNo { get; set; }
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string UserStatus { get; set; }
        public string UserId { get; set; }
        public PinUpdateType UpdateType { get; set; }
        public string UpdateTypeFormat => Enum.GetName(typeof(PinUpdateType), UpdateType);
        public string APIResponseCode { get; set; }
        public string APIResponseMessage { get; set; }
        public string AuthStatusFormat => Enum.GetName(typeof(AuthStatus), AuthStatus);
    }
}
