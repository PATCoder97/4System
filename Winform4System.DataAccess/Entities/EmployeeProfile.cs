using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("hr_EmployeeProfile")]
    public sealed class EmployeeProfile
    {
        [Key]
        public long EmployeeProfileId { get; set; }

        [Required, StringLength(200)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string PreferredName { get; set; }

        public int DepartmentId { get; set; }
    }
}
