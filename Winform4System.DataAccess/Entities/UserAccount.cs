using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_UserAccount")]
    public sealed class UserAccount
    {
        [Key]
        public long UserId { get; set; }

        public long? EmployeeProfileId { get; set; }

        [Required, StringLength(100)]
        public string LoginName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed), StringLength(100)]
        public string NormalizedLoginName { get; private set; }

        [Required, StringLength(20)]
        public string AuthenticationType { get; set; }

        [StringLength(500)]
        public string PasswordHash { get; set; }

        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
