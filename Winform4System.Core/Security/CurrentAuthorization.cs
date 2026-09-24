using System;
using System.Collections.Generic;
using System.Linq;

namespace Winform4System.Core.Security
{
    public static class CurrentAuthorization
    {
        private static readonly object SyncRoot = new object();
        private static HashSet<string> _permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static string _userId;

        public static string UserId
        {
            get { lock (SyncRoot) return _userId; }
        }

        public static void SetIdentityAndPermissions(string userId, IEnumerable<string> permissionCodes)
        {
            lock (SyncRoot)
            {
                _userId = string.IsNullOrWhiteSpace(userId) ? null : userId.Trim().ToUpperInvariant();
                _permissions = new HashSet<string>(permissionCodes ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            }
        }

        public static void SetPermissions(IEnumerable<string> permissionCodes)
        {
            SetIdentityAndPermissions(null, permissionCodes);
        }

        public static bool HasPermission(string permissionCode)
        {
            lock (SyncRoot)
            {
                if (_permissions.Contains(permissionCode))
                    return true;

                return !string.IsNullOrWhiteSpace(permissionCode)
                    && permissionCode.StartsWith("ASSET.SPARE_PART.", StringComparison.OrdinalIgnoreCase)
                    && _permissions.Contains("ASSET.SPARE_PART.ADMIN");
            }
        }

        public static void Demand(string permissionCode)
        {
            if (!HasPermission(permissionCode))
                throw new UnauthorizedAccessException("您沒有執行此操作的權限。");
        }
    }
}
