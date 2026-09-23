using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_RolePermission")]
    public sealed class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool IsActive { get; set; }
    }
}
