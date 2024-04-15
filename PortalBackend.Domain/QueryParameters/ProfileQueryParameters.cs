﻿using 
    PortalBackend.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace PortalBackend.Domain.QueryParameters
{
    public class ProfileQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Text)]
        public string Query { get; set; } 
    }
}
