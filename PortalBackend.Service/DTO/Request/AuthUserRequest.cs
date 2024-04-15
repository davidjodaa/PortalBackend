namespace PortalBackend.Service.DTO.Request
{
    public class AuthUserRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string key { get; set; }
        public string channel { get; set; }
    }
}