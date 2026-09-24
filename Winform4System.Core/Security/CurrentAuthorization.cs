using System;
using System.Collections.Generic;
using System.Linq;

namespace Winform4System.Core.Security
{
    public static class CurrentAuthorization
    {
        private static readonly object SyncRoot = new object();
        private static HashSet<string> _permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static void SetPermissions(IEnumerable<string> permissionCodes)
        {
            lock (SyncRoot)
            {
                _permissions = new HashSet<string>(
                    permissionCodes ?? Enumerable.Empty<string>(),
                    StringComparer.OrdinalIgnoreCase);
            }
        }

        public static bool HasPermission(string permissionCode)
        {
            lock (SyncRoot)
            {
                return _permissions.Contains(permissionCode)
                    || _permissions.Contains("ASSET.SPARE_PART.ADMIN");
            }
        }

        public static void Demand(string permissionCode)
        {
            if (!HasPermission(permissionCode))
                throw new UnauthorizedAccessException("您沒有執行此操作的權限。");
        }
    }
}
