using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_RolePermission")]
    public sealed class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime AssignedAt { get; set; }

        [StringLength(10)]
        public string AssignedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
