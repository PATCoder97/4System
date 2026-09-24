using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("dm_JobTitle")]
    public sealed class JobTitle
    {
        [Key]
        public int JobTitleId { get; set; }

        [Required, StringLength(30)]
        public string JobTitleCode { get; set; }

        [Required, StringLength(200)]
        public string JobTitleName { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
