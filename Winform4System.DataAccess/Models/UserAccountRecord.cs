using System;

namespace Winform4System.DataAccess.Models
{
    public sealed class UserAccountRecord
    {
        public string UserId { get; set; }
        public string CachedDomainPasswordHash { get; set; }
        public DateTime? LastDomainValidatedAt { get; set; }
        public Guid SecurityStamp { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public string Department { get; set; }
        public string DepartmentCode { get; set; }
        public string Roles { get; set; }
        public string[] PermissionCodes { get; set; }
    }
}
