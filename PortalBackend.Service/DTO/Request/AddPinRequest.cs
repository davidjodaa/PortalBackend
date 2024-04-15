using PortalBackend.Domain.Constants;
using PortalBackend.Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Service.DTO.Request
{
    public class AddPinRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public string AccountNo { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string CustomerId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Mobile { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Email { get; set; }
        [DataType(DataType.Text)]
        public string UserStatus { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public PinUpdateType UpdateType { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Authorizer { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile SupportingDocument { get; set; }
    }

    public class AddPinRequestValidator : AbstractValidator<AddPinRequest>
    {
        public AddPinRequestValidator()
        {
            //RuleFor(o => o.Id).GreaterThanOrEqualTo(0).WithMessage("ID is not valid");
            //RuleFor(o => o.CommisionPercent).GreaterThanOrEqualTo(0).WithMessage("Commission percent is not valid");
            //RuleFor(o => o.MaxAmount).GreaterThanOrEqualTo(o => o.MinAmount).WithMessage("Maximum amount is not valid");
            //RuleFor(o => o.MinAmount).GreaterThanOrEqualTo(0).WithMessage("Minimum amount is not valid");
            //RuleFor(o => o.Charges).GreaterThan(0).WithMessage("Charge is not valid");
            //RuleFor(o => o.TransactionType).NotEmpty().Must(x => x.Trim(GeneralConstants.EXCLUDED_CHARACTERS) == x).WithMessage("Transaction type is not valid");
            //RuleFor(o => o.VATRate).GreaterThan(0).WithMessage("VAT Rate is not valid");
        }
    }
}
