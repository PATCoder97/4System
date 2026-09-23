using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_GroupRole")]
    public sealed class GroupRole
    {
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
