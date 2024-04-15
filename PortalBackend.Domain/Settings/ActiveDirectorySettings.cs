
namespace PortalBackend.Domain.Settings
{
    public class ActiveDirectorySettings
    {
        public string LdapIp { get; set; }
        public string LdapCred { get; set; }
        public string LdapPath { get; set; }
        public string AdUserName { get; set; }
        public string AdPassword { get; set; }
        public string DomainUser { get; set; }
    }
}
