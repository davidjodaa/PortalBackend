
namespace PortalBackend.Service.DTO.Response
{
    public class FindUserResponse
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string source_code { get; set; }
        public string pin_status { get; set; }
        public string encrypted_pin { get; set; }
        public string response_time { get; set; }
    }
}
