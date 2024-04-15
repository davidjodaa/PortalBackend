using PortalBackend.Domain.Common;
using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Extension;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Domain.QueryParameters
{
    public class AcctOpenRptQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Text)]
        public string Query { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string AccountNumber { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string StartDate { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string EndDate { get; set; }
    }
    public class AcctOpenRptQueryParametersValidator : AbstractValidator<AcctOpenRptQueryParameters>
    {
        public AcctOpenRptQueryParametersValidator()
        {
            RuleFor(o => o.Query)
                .NotEmpty()
                .Must(x => x.Trim(GeneralConstants.EXCLUDED_CHARACTERS) == x)
                .When(o => !string.IsNullOrWhiteSpace(o.Query))
                .WithMessage("Query is invalid");
            RuleFor(o => o.AccountNumber).NotEmpty().Must(x => string.IsNullOrEmpty(x.Trim(GeneralConstants.NUMBERS))).WithMessage("Account Number is invalid");
            RuleFor(o => o.StartDate).NotEmpty().Must(x => x.IsValidDate()).WithMessage("Start date is invalid");
            RuleFor(o => o.EndDate).NotEmpty().Must(x => x.IsValidDate()).WithMessage("End date is invalid");
        }
    }
}
