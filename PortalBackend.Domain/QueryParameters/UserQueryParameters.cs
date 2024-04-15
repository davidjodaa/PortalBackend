using PortalBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Domain.QueryParameters
{
    public class UserQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Text)]
        public string Query { get; set; }
        [DataType(DataType.Text)]
        public string Role { get; set; }
        [DataType(DataType.Text)]
        public int Status { get; set; }
    }
}
