using PortalBackend.Domain;
using PortalBackend.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Service.DTO.Response
{
    public class ProfileAuditResponse : AuthorizableEntity
    {
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public ProfileStatus ProfileStatus { get; set; }
        public string ProfileStatusFormat => Enum.GetName(typeof(ProfileStatus), ProfileStatus);
        public string AuthStatusFormat => Enum.GetName(typeof(AuthStatus), AuthStatus);
        public string APIResponseCode { get; set; }
        public string APIResponseMessage { get; set; }
    }
}
