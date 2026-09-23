using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_Role")]
    public sealed class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, StringLength(200)]
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
    }
}
