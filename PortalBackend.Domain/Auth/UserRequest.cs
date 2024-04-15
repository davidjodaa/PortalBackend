using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Domain.Auth
{
    public class UserRequest
    {
        [Required]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string StaffGroup { get; set; }
        public string StaffBranch { get; set; }
        public string Role { get; set; }
    }
}
