using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("audit_AuditLog")]
    public sealed class AuditLog
    {
        [Key]
        public long AuditLogId { get; set; }
        [StringLength(10)]
        public string UserId { get; set; }

        [StringLength(100)]
        public string AttemptedUserId { get; set; }

        [Required, StringLength(100)]
        public string ActionCode { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string MachineName { get; set; }
    }
}
