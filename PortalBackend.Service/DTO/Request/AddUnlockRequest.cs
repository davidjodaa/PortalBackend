using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Service.DTO.Request
{
    public class AddUnlockRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public string NewUser { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string CurrentUser { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Uuid { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Authorizer { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile SupportingDocument { get; set; }
    }
}
