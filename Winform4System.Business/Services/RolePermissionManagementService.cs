using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class RolePermissionManagementService
    {
        private static readonly Regex CodePattern = new Regex("^[A-Z][A-Z0-9_]{2,79}$", RegexOptions.Compiled);
        private static readonly string[] Actions = { "ACCESS", "VIEW", "CREATE", "UPDATE", "DELETE", "APPROVE", "EXPORT", "ADMIN" };
        private readonly string _connectionString;

        public RolePermissionManagementService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public IReadOnlyList<RoleListItem> GetRoles()
        {
            CurrentAuthorization.Demand("SYSTEM.ROLE.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.Roles.AsNoTracking().OrderBy(x => x.RoleCode)
                    .Select(role => new RoleListItem
                    {
                        RoleId = role.RoleId,
                        RoleCode = role.RoleCode,
                        RoleName = role.RoleName,
                        Description = role.Description,
                        IsSystemRole = role.IsSystemRole,
                        IsActive = role.IsActive,
                        PermissionCount = context.RolePermissions.Count(x => x.RoleId == role.RoleId && x.IsActive),
                        GroupCount = context.GroupRoles.Count(x => x.RoleId == role.RoleId && x.IsActive),
                        RowVersion = role.RowVersion
                    }).ToList();
            }
        }

        public RolePermissionDetail GetDetail(int? roleId)
        {
            CurrentAuthorization.Demand("SYSTEM.ROLE.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                var selectedIds = roleId.HasValue
                    ? new HashSet<int>(context.RolePermissions.AsNoTracking().Where(x => x.RoleId == roleId.Value && x.IsActive).Select(x => x.PermissionId))
                    : new HashSet<int>();
                var functions = context.ApplicationFunctions.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.DisplayName).ToList();
                var permissions = context.Permissions.AsNoTracking().Where(x => x.IsActive).ToList();
                var matrix = functions.Select(function => CreateMatrixRow(function, permissions.Where(x => x.FunctionId == function.FunctionId), selectedIds)).ToList();

                var groups = roleId.HasValue
                    ? (from mapping in context.GroupRoles.AsNoTracking()
                       join securityGroup in context.SecurityGroups.AsNoTracking() on mapping.GroupId equals securityGroup.GroupId
                       where mapping.RoleId == roleId.Value && mapping.IsActive
                       orderby securityGroup.GroupName
                       select new AffectedGroupItem { GroupId = securityGroup.GroupId, GroupCode = securityGroup.GroupCode, GroupName = securityGroup.GroupName, IsActive = securityGroup.IsActive }).ToList()
                    : new List<AffectedGroupItem>();
                var users = roleId.HasValue
                    ? (from groupRole in context.GroupRoles.AsNoTracking()
                       join userGroup in context.UserGroups.AsNoTracking() on groupRole.GroupId equals userGroup.GroupId
                       join account in context.UserAccounts.AsNoTracking() on userGroup.UserId equals account.UserId
                       join employee in context.EmployeeProfiles.AsNoTracking() on account.EmployeeProfileId equals employee.EmployeeProfileId
                       where groupRole.RoleId == roleId.Value && groupRole.IsActive && userGroup.IsActive && (!userGroup.ExpiresAt.HasValue || userGroup.ExpiresAt > DateTime.UtcNow)
                       select new AffectedUserItem { UserId = account.UserId, DisplayNameTW = employee.DisplayNameTW, DisplayNameVN = employee.DisplayNameVN, IsAccountActive = account.IsActive }).ToList()
                       .GroupBy(x => x.UserId, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).OrderBy(x => x.UserId).ToList()
                    : new List<AffectedUserItem>();
                return new RolePermissionDetail { Matrix = matrix, Groups = groups, Users = users };
            }
        }

        public int Save(RoleEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.ROLE.ADMIN");
            Validate(model);
            string code = model.RoleCode.Trim().ToUpperInvariant();
            var selectedIds = new HashSet<int>(model.PermissionIds ?? Enumerable.Empty<int>());
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                Role role;
                bool isNew = !model.RoleId.HasValue;
                if (isNew)
                {
                    if (context.Roles.Any(x => x.RoleCode == code)) throw new InvalidOperationException("此角色代碼已存在。");
                    role = new Role { RoleCode = code, IsSystemRole = false, IsActive = true };
                    context.Roles.Add(role);
                }
                else
                {
                    role = context.Roles.FirstOrDefault(x => x.RoleId == model.RoleId.Value);
                    if (role == null) throw new InvalidOperationException("找不到需要更新的角色。");
                    if (!role.RowVersion.SequenceEqual(model.RowVersion ?? new byte[0])) throw new InvalidOperationException("此角色已由其他使用者更新，請重新載入後再試。");
                    if (!string.Equals(role.RoleCode, code, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("建立後不可變更角色代碼。");
                }

                var validIds = new HashSet<int>(context.Permissions.Where(x => x.IsActive).Select(x => x.PermissionId));
                if (selectedIds.Any(x => !validIds.Contains(x))) throw new InvalidOperationException("所選權限無效或已停用。");
                role.RoleName = model.RoleName.Trim(); role.Description = Normalize(model.Description); role.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();
                var mappings = context.RolePermissions.Where(x => x.RoleId == role.RoleId).ToList();
                var beforePermissionIds = new HashSet<int>(mappings.Where(x => x.IsActive).Select(x => x.PermissionId));
                var relevantPermissionIds = beforePermissionIds.Union(selectedIds).ToList();
                var permissionCodes = context.Permissions.Where(x => relevantPermissionIds.Contains(x.PermissionId)).ToDictionary(x => x.PermissionId, x => x.PermissionCode);
                foreach (var mapping in mappings) mapping.IsActive = selectedIds.Contains(mapping.PermissionId);
                foreach (int permissionId in selectedIds.Where(x => mappings.All(m => m.PermissionId != x)))
                    context.RolePermissions.Add(new RolePermission { RoleId = role.RoleId, PermissionId = permissionId, AssignedAt = DateTime.UtcNow, AssignedByUserId = CurrentAuthorization.UserId, IsActive = true });
                var audit = CreateAudit(isNew ? "SECURITY.ROLE.CREATE" : "SECURITY.ROLE.UPDATE", role.RoleId.ToString(), (isNew ? "建立" : "更新") + "角色 " + code);
                audit.DataJson = AuditDataJson.CollectionChange("permissions", beforePermissionIds.Select(x => permissionCodes[x]), selectedIds.Select(x => permissionCodes[x]));
                context.AuditLogs.Add(audit);
                context.SaveChanges(); transaction.Commit(); return role.RoleId;
            }
        }

        public void Deactivate(int roleId, byte[] rowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.ROLE.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                var role = context.Roles.FirstOrDefault(x => x.RoleId == roleId);
                if (role == null) throw new InvalidOperationException("找不到所選角色。");
                if (!role.RowVersion.SequenceEqual(rowVersion ?? new byte[0])) throw new InvalidOperationException("此角色已由其他使用者更新，請重新載入後再試。");
                bool hasGroups = context.GroupRoles.Any(x => x.RoleId == roleId && x.IsActive);
                if (role.IsSystemRole && hasGroups) throw new InvalidOperationException("系統角色仍指派給安全性群組，無法停用。請先移轉群組角色。");
                bool wasActive = role.IsActive;
                role.IsActive = false; role.UpdatedAt = DateTime.UtcNow;
                var audit = CreateAudit("SECURITY.ROLE.DEACTIVATE", role.RoleId.ToString(), "停用角色 " + role.RoleCode);
                audit.DataJson = AuditDataJson.Change(new Dictionary<string, string> { { "isActive", wasActive.ToString() } }, new Dictionary<string, string> { { "isActive", "False" } });
                context.AuditLogs.Add(audit);
                context.SaveChanges(); transaction.Commit();
            }
        }

        private static PermissionMatrixRow CreateMatrixRow(ApplicationFunction function, IEnumerable<Permission> permissions, HashSet<int> selectedIds)
        {
            var row = new PermissionMatrixRow { FunctionId = function.FunctionId, FunctionCode = function.FunctionCode, FunctionName = function.DisplayName };
            foreach (var permission in permissions.Where(x => Actions.Contains(x.ActionCode))) row.Set(permission.ActionCode, permission.PermissionId, selectedIds.Contains(permission.PermissionId));
            return row;
        }
        private static void Validate(RoleEditModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!CodePattern.IsMatch((model.RoleCode ?? string.Empty).Trim().ToUpperInvariant())) throw new InvalidOperationException("角色代碼須以英文字母開頭，且只能包含大寫英文字母、數字及底線。");
            if (string.IsNullOrWhiteSpace(model.RoleName)) throw new InvalidOperationException("請輸入角色名稱。");
            if (model.RoleName.Trim().Length > 200) throw new InvalidOperationException("角色名稱不可超過 200 個字元。");
            if (!string.IsNullOrWhiteSpace(model.Description) && model.Description.Trim().Length > 500) throw new InvalidOperationException("角色說明不可超過 500 個字元。");
        }
        private static AuditLog CreateAudit(string action, string id, string description) => new AuditLog { UserId = CurrentAuthorization.UserId, ActionCode = action, EntityName = "auth_Role", EntityId = id, Description = description, MachineName = Environment.MachineName };
        private static string Normalize(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public sealed class RoleListItem { public int RoleId { get; set; } public string RoleCode { get; set; } public string RoleName { get; set; } public string Description { get; set; } public bool IsSystemRole { get; set; } public bool IsActive { get; set; } public int PermissionCount { get; set; } public int GroupCount { get; set; } public byte[] RowVersion { get; set; } }
    public sealed class RoleEditModel { public int? RoleId { get; set; } public string RoleCode { get; set; } public string RoleName { get; set; } public string Description { get; set; } public IReadOnlyList<int> PermissionIds { get; set; } public byte[] RowVersion { get; set; } }
    public sealed class RolePermissionDetail { public IReadOnlyList<PermissionMatrixRow> Matrix { get; set; } public IReadOnlyList<AffectedGroupItem> Groups { get; set; } public IReadOnlyList<AffectedUserItem> Users { get; set; } }
    public sealed class AffectedGroupItem { public int GroupId { get; set; } public string GroupCode { get; set; } public string GroupName { get; set; } public bool IsActive { get; set; } }
    public sealed class AffectedUserItem { public string UserId { get; set; } public string DisplayNameTW { get; set; } public string DisplayNameVN { get; set; } public bool IsAccountActive { get; set; } }

    public sealed class PermissionMatrixRow
    {
        public int FunctionId { get; set; } public string FunctionCode { get; set; } public string FunctionName { get; set; }
        public int? AccessId { get; set; } public bool? Access { get; set; }
        public int? ViewId { get; set; } public bool? View { get; set; }
        public int? CreateId { get; set; } public bool? Create { get; set; }
        public int? UpdateId { get; set; } public bool? Update { get; set; }
        public int? DeleteId { get; set; } public bool? Delete { get; set; }
        public int? ApproveId { get; set; } public bool? Approve { get; set; }
        public int? ExportId { get; set; } public bool? Export { get; set; }
        public int? AdminId { get; set; } public bool? Admin { get; set; }
        public void Set(string action, int id, bool selected) { switch (action) { case "ACCESS": AccessId = id; Access = selected; break; case "VIEW": ViewId = id; View = selected; break; case "CREATE": CreateId = id; Create = selected; break; case "UPDATE": UpdateId = id; Update = selected; break; case "DELETE": DeleteId = id; Delete = selected; break; case "APPROVE": ApproveId = id; Approve = selected; break; case "EXPORT": ExportId = id; Export = selected; break; case "ADMIN": AdminId = id; Admin = selected; break; } }
        public IEnumerable<int> GetSelectedIds() { if (Access == true && AccessId.HasValue) yield return AccessId.Value; if (View == true && ViewId.HasValue) yield return ViewId.Value; if (Create == true && CreateId.HasValue) yield return CreateId.Value; if (Update == true && UpdateId.HasValue) yield return UpdateId.Value; if (Delete == true && DeleteId.HasValue) yield return DeleteId.Value; if (Approve == true && ApproveId.HasValue) yield return ApproveId.Value; if (Export == true && ExportId.HasValue) yield return ExportId.Value; if (Admin == true && AdminId.HasValue) yield return AdminId.Value; }
        public bool HasPermission(string fieldName) { switch (fieldName) { case "Access": return AccessId.HasValue; case "View": return ViewId.HasValue; case "Create": return CreateId.HasValue; case "Update": return UpdateId.HasValue; case "Delete": return DeleteId.HasValue; case "Approve": return ApproveId.HasValue; case "Export": return ExportId.HasValue; case "Admin": return AdminId.HasValue; default: return false; } }
    }
}
