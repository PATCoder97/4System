using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_SecurityGroup")]
    public sealed class SecurityGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required, StringLength(80)]
        public string GroupCode { get; set; }

        [Required, StringLength(200)]
        public string GroupName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsSystemGroup { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
