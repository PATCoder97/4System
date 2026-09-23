using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_SecurityGroup")]
    public sealed class SecurityGroup
    {
        [Key]
        public int GroupId { get; set; }
        public bool IsActive { get; set; }
    }
}
