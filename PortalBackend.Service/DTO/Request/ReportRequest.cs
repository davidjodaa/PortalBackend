using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Service.DTO.Request
{
    public class ReportRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public string StartDate { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string EndDate { get; set; }
    }

    public class ReportRequestValidator : AbstractValidator<ReportRequest>
    {
        public ReportRequestValidator()
        {
            RuleFor(o => o.StartDate).Must(x => DateTime.TryParse(x, out var val) && val < DateTime.Now).WithMessage("Invalid Date.");
            RuleFor(o => o.EndDate).Must(x => DateTime.TryParse(x, out var val)).WithMessage("Invalid Date.");
        }
    }
}
