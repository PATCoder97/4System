using System;

namespace Winform4System.DataAccess.Models
{
    public sealed class UserAccountRecord
    {
        public long UserId { get; set; }
        public string LoginName { get; set; }
        public string AuthenticationType { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Roles { get; set; }
    }
}
