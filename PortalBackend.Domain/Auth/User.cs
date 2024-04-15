using PortalBackend.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Domain.Auth
{
    public class User : BaseEntity
    {
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("MOBILE_NUMBER")]
        public string MobileNumber { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("BRANCH_CODE")]
        public string Branch { get; set; }
        [Column("GROUP")]
        public string Group { get; set; }
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
        [Column("IS_LOGGED_IN")]
        public bool IsLoggedIn { get; set; }
        [Column("LAST_LOGIN_TIME")]
        public DateTime LastLoginTime { get; set; }
        [Column("ROLE_ID")]
        public string RoleId { get; set; }
    }
}