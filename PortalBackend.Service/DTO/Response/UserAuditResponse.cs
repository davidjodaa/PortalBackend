using PortalBackend.Domain;
using PortalBackend.Domain.Enum;
using System;

namespace PortalBackend.Service.DTO.Response
{
    public class UserAuditResponse : AuthorizableEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Group { get; set; }
        public string RoleId { get; set; }
        public UserRequestType RequestType { get; set; }
        public string RequestTypeFormat => Enum.GetName(typeof(UserRequestType), RequestType);
        public string AuthStatusFormat => Enum.GetName(typeof(AuthStatus), AuthStatus);
    }
}
