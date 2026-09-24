using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_Permission")]
    public sealed class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public int FunctionId { get; set; }

        [Required, StringLength(30)]
        public string ActionCode { get; set; }

        [Required, StringLength(150)]
        public string PermissionCode { get; set; }

        [Required, StringLength(200)]
        public string DisplayName { get; set; }

        public bool IsActive { get; set; }
    }
}
