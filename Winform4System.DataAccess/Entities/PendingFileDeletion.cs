using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("sys_PendingFileDeletion")]
    public sealed class PendingFileDeletion
    {
        [Key]
        public long Id { get; set; }
        [Required, StringLength(1000)]
        public string RelativePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime NextAttemptAt { get; set; }
        public int AttemptCount { get; set; }
        [StringLength(1000)]
        public string LastError { get; set; }
    }
}
