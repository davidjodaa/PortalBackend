
using PortalBackend.Domain.Constants;
using FluentValidation;

namespace PortalBackend.Domain.Auth
{
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }

    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(o => o.Username).NotEmpty().Must(x => x.Trim(GeneralConstants.EXCLUDED_CHARACTERS) == x).WithMessage("Username is invalid");
            RuleFor(o => o.Password).NotEmpty().WithMessage("Password is invalid");
        }
    }
}
