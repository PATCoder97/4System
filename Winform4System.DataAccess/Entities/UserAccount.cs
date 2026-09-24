using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_UserAccount")]
    public sealed class UserAccount
    {
        [Key]
        [StringLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }

        public long? EmployeeProfileId { get; set; }

        [StringLength(500)]
        public string CachedDomainPasswordHash { get; set; }
        public DateTime? LastDomainValidatedAt { get; set; }
        public Guid SecurityStamp { get; set; }

        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
