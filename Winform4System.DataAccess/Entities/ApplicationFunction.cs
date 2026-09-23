using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("app_Function")]
    public sealed class ApplicationFunction
    {
        [Key]
        public int FunctionId { get; set; }
        public int? ParentFunctionId { get; set; }

        [Required, StringLength(100)]
        public string FunctionCode { get; set; }

        [Required, StringLength(200)]
        public string DisplayName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(300)]
        public string NavigationTarget { get; set; }

        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }

        [Required, StringLength(20)]
        public string DevelopmentStatus { get; set; }

        public bool IsWide { get; set; }
    }
}
