using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_Role")]
    public sealed class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, StringLength(80)]
        public string RoleCode { get; set; }

        [Required, StringLength(200)]
        public string RoleName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
