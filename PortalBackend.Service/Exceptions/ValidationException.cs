using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortalBackend.Service.Exceptions
{
    public class InvalidException : Exception
    {
        public InvalidException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new List<string>();
        }

        public InvalidException(IEnumerable<ModelError> failures)
            : this()
        {
            Failures = failures.Select(e => e.ErrorMessage).ToList();
        }

        public List<string> Failures { get; }
    }
}
