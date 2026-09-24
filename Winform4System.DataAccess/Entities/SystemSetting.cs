using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("sys_SystemSetting")]
    public sealed class SystemSetting
    {
        [Key, StringLength(100)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SettingKey { get; set; }

        [Required, StringLength(1000)]
        public string SettingValue { get; set; }

        [Required, StringLength(200)]
        public string DisplayName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required, StringLength(20)]
        public string ValueType { get; set; }

        public int? MinimumValue { get; set; }
        public int? MaximumValue { get; set; }
        public bool IsSensitive { get; set; }

        [StringLength(10)]
        public string UpdatedByUserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
