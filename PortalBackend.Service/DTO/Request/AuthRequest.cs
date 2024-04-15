using PortalBackend.Domain.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Request
{
    public class AuthRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public long Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public AuthStatus AuthStatus { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string AuthorizersComment { get; set; }
    }

    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator()
        {
            RuleFor(o => o.Id).NotEmpty().WithMessage("ID is invalid");
            RuleFor(o => o.AuthStatus).IsInEnum().WithMessage("Authentication status is invalid");
            RuleFor(o => o.AuthorizersComment).NotEmpty().WithMessage("User role is invalid");
        }
    }
}
