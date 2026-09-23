using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_UserGroup")]
    public sealed class UserGroup
    {
        [StringLength(10)]
        public string UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
