using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalBackend.Domain.Auth
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string RoleId { get; set; }
        public string JWToken { get; set; }
        public double ExpiresIn { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
