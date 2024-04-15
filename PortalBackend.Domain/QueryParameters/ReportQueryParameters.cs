using PortalBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Domain.QueryParameters
{
    public class ReportQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Text)]
        public string StartDate { get; set; }
        [DataType(DataType.Text)]
        public string EndDate { get; set; }
    }
}
