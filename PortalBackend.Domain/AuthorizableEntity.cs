using PortalBackend.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Domain
{
    public class AuthorizableEntity : BaseEntity
    {
        [Column("INITIATOR")]
        public string Initiator { get; set; }
        [Column("INITIATOR_EMAIL")]
        public string InitiatorEmail { get; set; }
        [Column("DATE_INITIATED")]
        public DateTime DateInitiated { get; set; }
        [Column("AUTHORIZER")]
        public string Authorizer { get; set; }
        [Column("AUTHORIZER_EMAIL")]
        public string AuthorizerEmail { get; set; }
        [Column("DATE_AUTHORIZED")]
        public DateTime? DateAuthorized { get; set; }
        [Column("INITIATING_BRANCH")]
        public string InitiatingBranch { get; set; }
        [Column("AUTHORIZERS_COMMENT")]
        public string AuthorizersComment { get; set; }
        [Column("AUTH_STATUS")]
        public AuthStatus AuthStatus { get; set; }
    }
}
