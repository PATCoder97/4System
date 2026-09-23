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

        public bool IsActive { get; set; }
    }
}
