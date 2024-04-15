using PortalBackend.Domain;
using PortalBackend.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Service.DTO.Response
{
    public class DeviceAuditResponse : AuthorizableEntity
    {

        public string NewUser { get; set; }
        public string CurrentUser { get; set; }
        public string Uuid { get; set; }
        public string APIResponseCode { get; set; }
        public string APIResponseMessage { get; set; }
        public string AuthStatusFormat => Enum.GetName(typeof(AuthStatus), AuthStatus);
    }
}
