using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class EmployeePermissionService
    {
        private readonly string _connectionString;
        public EmployeePermissionService(ConnectionStringProvider provider) { _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get(); }

        public IReadOnlyList<EmployeeListItem> GetEmployees()
        {
            CurrentAuthorization.Demand("SYSTEM.USER.PERMISSION.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return (from account in context.UserAccounts.AsNoTracking()
                        join employee in context.EmployeeProfiles.AsNoTracking() on account.EmployeeProfileId equals employee.EmployeeProfileId
                        join department in context.Departments.AsNoTracking() on employee.DepartmentId equals department.DepartmentId
                        orderby employee.EmployeeCode
                        select new EmployeeListItem { EmployeeProfileId = employee.EmployeeProfileId, UserId = account.UserId, DisplayNameTW = employee.DisplayNameTW, DisplayNameVN = employee.DisplayNameVN, DepartmentName = department.DepartmentName, IsAccountActive = account.IsActive }).ToList();
            }
        }

        public EmployeePermissionDetail GetDetail(string userId)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.PERMISSION.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                var selectedIds = context.UserGroups.AsNoTracking().Where(x => x.UserId == userId && x.IsActive && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow)).Select(x => x.GroupId).ToList();
                var groups = context.SecurityGroups.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.GroupName).ToList()
                    .Select(x => new SecurityGroupOption { GroupId = x.GroupId, GroupCode = x.GroupCode, GroupName = x.GroupName, Description = x.Description, IsAssigned = selectedIds.Contains(x.GroupId) }).ToList();
                var roles = (from ug in context.UserGroups.AsNoTracking()
                             join gr in context.GroupRoles.AsNoTracking() on ug.GroupId equals gr.GroupId
                             join role in context.Roles.AsNoTracking() on gr.RoleId equals role.RoleId
                             where ug.UserId == userId && ug.IsActive && gr.IsActive && role.IsActive && (!ug.ExpiresAt.HasValue || ug.ExpiresAt > DateTime.UtcNow)
                             select role.RoleName).Distinct().OrderBy(x => x).ToList();
                var permissions = (from ug in context.UserGroups.AsNoTracking()
                                   join gr in context.GroupRoles.AsNoTracking() on ug.GroupId equals gr.GroupId
                                   join rp in context.RolePermissions.AsNoTracking() on gr.RoleId equals rp.RoleId
                                   join permission in context.Permissions.AsNoTracking() on rp.PermissionId equals permission.PermissionId
                                   where ug.UserId == userId && ug.IsActive && gr.IsActive && rp.IsActive && permission.IsActive && (!ug.ExpiresAt.HasValue || ug.ExpiresAt > DateTime.UtcNow)
                                   select new EffectivePermissionItem { PermissionCode = permission.PermissionCode, DisplayName = permission.DisplayName }).ToList()
                                   .GroupBy(x => x.PermissionCode, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).OrderBy(x => x.PermissionCode).ToList();
                return new EmployeePermissionDetail { Groups = groups, Roles = roles, Permissions = permissions };
            }
        }

        public void SaveGroups(string userId, IEnumerable<int> groupIds)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.PERMISSION.ADMIN");
            var selected = new HashSet<int>(groupIds ?? Enumerable.Empty<int>());
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                if (!context.UserAccounts.Any(x => x.UserId == userId)) throw new InvalidOperationException("找不到所選人員帳號。");
                var validIds = new HashSet<int>(context.SecurityGroups.Where(x => x.IsActive).Select(x => x.GroupId));
                if (selected.Any(x => !validIds.Contains(x))) throw new InvalidOperationException("所選安全性群組無效。");
                var mappings = context.UserGroups.Where(x => x.UserId == userId).ToList();
                foreach (var mapping in mappings) mapping.IsActive = selected.Contains(mapping.GroupId);
                foreach (int groupId in selected.Where(x => mappings.All(m => m.GroupId != x)))
                    context.UserGroups.Add(new UserGroup { UserId = userId, GroupId = groupId, IsActive = true });
                context.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public sealed class EmployeePermissionDetail
    {
        public IReadOnlyList<SecurityGroupOption> Groups { get; set; }
        public IReadOnlyList<string> Roles { get; set; }
        public IReadOnlyList<EffectivePermissionItem> Permissions { get; set; }
    }
    public sealed class SecurityGroupOption
    {
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsAssigned { get; set; }
    }
    public sealed class EffectivePermissionItem
    {
        public string PermissionCode { get; set; }
        public string DisplayName { get; set; }
    }
}
