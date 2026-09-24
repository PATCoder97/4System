using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("hr_EmployeeProfile")]
    public sealed class EmployeeProfile
    {
        [Key]
        public long EmployeeProfileId { get; set; }

        [Required, StringLength(10)]
        public string EmployeeCode { get; set; }

        [StringLength(100)]
        public string DisplayNameTW { get; set; }

        [Required, StringLength(200)]
        public string DisplayNameVN { get; set; }

        public int DepartmentId { get; set; }
        public int? JobTitleId { get; set; }

        [StringLength(200)]
        public string WorkEmail { get; set; }

        [StringLength(30)]
        public string WorkPhone { get; set; }

        public DateTime? HireDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public byte EmploymentStatus { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
