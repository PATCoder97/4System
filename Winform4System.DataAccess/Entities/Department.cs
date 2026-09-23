using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("dm_Department")]
    public sealed class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required, StringLength(200)]
        public string DepartmentName { get; set; }
    }
}
