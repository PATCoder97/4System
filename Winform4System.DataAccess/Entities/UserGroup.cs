using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Winform4System.DataAccess.Entities
{
    [Table("auth_UserGroup")]
    public sealed class UserGroup
    {
        public long UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
