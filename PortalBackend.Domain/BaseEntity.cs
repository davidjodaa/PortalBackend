using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalBackend.Domain
{
    public class BaseEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }
    }
}
