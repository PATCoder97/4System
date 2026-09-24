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
    public sealed class SecurityGroupManagementService
    {
        private static readonly Regex CodePattern = new Regex("^[A-Z][A-Z0-9_]{2,79}$", RegexOptions.Compiled);
        private readonly string _connectionString;

        public SecurityGroupManagementService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public IReadOnlyList<SecurityGroupListItem> GetGroups()
        {
            CurrentAuthorization.Demand("SYSTEM.GROUP.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.SecurityGroups.AsNoTracking()
                    .OrderBy(x => x.GroupCode)
                    .Select(group => new SecurityGroupListItem
                    {
                        GroupId = group.GroupId,
                        GroupCode = group.GroupCode,
                        GroupName = group.GroupName,
                        Description = group.Description,
                        IsSystemGroup = group.IsSystemGroup,
                        IsActive = group.IsActive,
                        RoleCount = context.GroupRoles.Count(x => x.GroupId == group.GroupId && x.IsActive),
                        MemberCount = context.UserGroups.Count(x => x.GroupId == group.GroupId && x.IsActive && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow)),
                        RowVersion = group.RowVersion
                    }).ToList();
            }
        }

        public SecurityGroupDetail GetDetail(int? groupId)
        {
            CurrentAuthorization.Demand("SYSTEM.GROUP.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                var selectedRoleIds = groupId.HasValue
                    ? context.GroupRoles.AsNoTracking().Where(x => x.GroupId == groupId.Value && x.IsActive).Select(x => x.RoleId).ToList()
                    : new List<int>();
                var roles = context.Roles.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.RoleName).ToList()
                    .Select(x => new SecurityGroupRoleOption { RoleId = x.RoleId, RoleCode = x.RoleCode, RoleName = x.RoleName, Description = x.Description, IsAssigned = selectedRoleIds.Contains(x.RoleId) }).ToList();
                var members = groupId.HasValue
                    ? (from mapping in context.UserGroups.AsNoTracking()
                       join account in context.UserAccounts.AsNoTracking() on mapping.UserId equals account.UserId
                       join employee in context.EmployeeProfiles.AsNoTracking() on account.EmployeeProfileId equals employee.EmployeeProfileId
                       join department in context.Departments.AsNoTracking() on employee.DepartmentId equals department.DepartmentId
                       where mapping.GroupId == groupId.Value && mapping.IsActive && (!mapping.ExpiresAt.HasValue || mapping.ExpiresAt > DateTime.UtcNow)
                       orderby employee.EmployeeCode
                       select new SecurityGroupMemberItem { UserId = account.UserId, DisplayNameTW = employee.DisplayNameTW, DisplayNameVN = employee.DisplayNameVN, DepartmentName = department.DepartmentName, IsAccountActive = account.IsActive, ExpiresAt = mapping.ExpiresAt }).ToList()
                    : new List<SecurityGroupMemberItem>();
                return new SecurityGroupDetail { Roles = roles, Members = members };
            }
        }

        public int Save(SecurityGroupEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.GROUP.ADMIN");
            Validate(model);
            string code = model.GroupCode.Trim().ToUpperInvariant();
            var selectedRoles = new HashSet<int>(model.RoleIds ?? Enumerable.Empty<int>());

            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                SecurityGroup group;
                bool isNew = !model.GroupId.HasValue;
                if (isNew)
                {
                    if (context.SecurityGroups.Any(x => x.GroupCode == code)) throw new InvalidOperationException("此群組代碼已存在。");
                    group = new SecurityGroup { GroupCode = code, IsSystemGroup = false, IsActive = true };
                    context.SecurityGroups.Add(group);
                }
                else
                {
                    group = context.SecurityGroups.FirstOrDefault(x => x.GroupId == model.GroupId.Value);
                    if (group == null) throw new InvalidOperationException("找不到需要更新的安全性群組。");
                    if (!group.RowVersion.SequenceEqual(model.RowVersion ?? new byte[0])) throw new InvalidOperationException("此群組已由其他使用者更新，請重新載入後再試。");
                    if (!string.Equals(group.GroupCode, code, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("建立後不可變更群組代碼。");
                }

                var validRoleIds = new HashSet<int>(context.Roles.Where(x => x.IsActive).Select(x => x.RoleId));
                if (selectedRoles.Any(x => !validRoleIds.Contains(x))) throw new InvalidOperationException("所選角色無效或已停用。");
                group.GroupName = model.GroupName.Trim();
                group.Description = Normalize(model.Description);
                group.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();

                var mappings = context.GroupRoles.Where(x => x.GroupId == group.GroupId).ToList();
                foreach (var mapping in mappings) mapping.IsActive = selectedRoles.Contains(mapping.RoleId);
                foreach (int roleId in selectedRoles.Where(x => mappings.All(m => m.RoleId != x)))
                    context.GroupRoles.Add(new GroupRole { GroupId = group.GroupId, RoleId = roleId, AssignedAt = DateTime.UtcNow, AssignedByUserId = CurrentAuthorization.UserId, IsActive = true });

                context.AuditLogs.Add(CreateAudit(isNew ? "SECURITY.GROUP.CREATE" : "SECURITY.GROUP.UPDATE", group.GroupId.ToString(), (isNew ? "建立" : "更新") + "安全性群組 " + code));
                context.SaveChanges();
                transaction.Commit();
                return group.GroupId;
            }
        }

        public void Deactivate(int groupId, byte[] rowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.GROUP.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                var group = context.SecurityGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (group == null) throw new InvalidOperationException("找不到所選安全性群組。");
                if (!group.RowVersion.SequenceEqual(rowVersion ?? new byte[0])) throw new InvalidOperationException("此群組已由其他使用者更新，請重新載入後再試。");
                bool hasMembers = context.UserGroups.Any(x => x.GroupId == groupId && x.IsActive && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow));
                if (group.IsSystemGroup && hasMembers) throw new InvalidOperationException("系統群組仍有使用者，無法停用。請先移轉群組成員。");
                group.IsActive = false;
                group.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAudit("SECURITY.GROUP.DEACTIVATE", group.GroupId.ToString(), "停用安全性群組 " + group.GroupCode));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void Validate(SecurityGroupEditModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            string code = (model.GroupCode ?? string.Empty).Trim().ToUpperInvariant();
            if (!CodePattern.IsMatch(code)) throw new InvalidOperationException("群組代碼須以英文字母開頭，且只能包含大寫英文字母、數字及底線。");
            if (string.IsNullOrWhiteSpace(model.GroupName)) throw new InvalidOperationException("請輸入群組名稱。");
            if (model.GroupName.Trim().Length > 200) throw new InvalidOperationException("群組名稱不可超過 200 個字元。");
            if (!string.IsNullOrWhiteSpace(model.Description) && model.Description.Trim().Length > 500) throw new InvalidOperationException("群組說明不可超過 500 個字元。");
        }

        private static AuditLog CreateAudit(string action, string entityId, string description)
        {
            return new AuditLog { UserId = CurrentAuthorization.UserId, ActionCode = action, EntityName = "auth_SecurityGroup", EntityId = entityId, Description = description, MachineName = Environment.MachineName };
        }

        private static string Normalize(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public sealed class SecurityGroupListItem
    {
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsSystemGroup { get; set; }
        public bool IsActive { get; set; }
        public int RoleCount { get; set; }
        public int MemberCount { get; set; }
        public byte[] RowVersion { get; set; }
    }
    public sealed class SecurityGroupEditModel
    {
        public int? GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<int> RoleIds { get; set; }
        public byte[] RowVersion { get; set; }
    }
    public sealed class SecurityGroupDetail
    {
        public IReadOnlyList<SecurityGroupRoleOption> Roles { get; set; }
        public IReadOnlyList<SecurityGroupMemberItem> Members { get; set; }
    }
    public sealed class SecurityGroupRoleOption
    {
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsAssigned { get; set; }
    }
    public sealed class SecurityGroupMemberItem
    {
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public string DepartmentName { get; set; }
        public bool IsAccountActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
