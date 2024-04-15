using PortalBackend.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class AddProfileRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string CustomerId { get; set; }
        [DataType(DataType.Text)]
        public string Mobile { get; set; }
        [DataType(DataType.Text)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public ProfileStatus ProfileStatus { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Authorizer { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile SupportingDocument { get; set; }
    }
}
