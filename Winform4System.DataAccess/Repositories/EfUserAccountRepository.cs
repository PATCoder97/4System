using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.DataAccess.Models;

namespace Winform4System.DataAccess.Repositories
{
    public sealed class EfUserAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public EfUserAccountRepository(ConnectionStringProvider connectionStringProvider)
        {
            if (connectionStringProvider == null)
                throw new ArgumentNullException(nameof(connectionStringProvider));

            _connectionString = connectionStringProvider.Get();
        }

        public EfUserAccountRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public UserAccountRecord FindByUserId(string userId)
        {
            using (var context = CreateContext())
            {
                var accountData =
                    (from user in context.UserAccounts.AsNoTracking()
                     where user.UserId == userId
                     join employee in context.EmployeeProfiles.AsNoTracking()
                         on user.EmployeeProfileId equals employee.EmployeeProfileId into employees
                     from employee in employees.DefaultIfEmpty()
                     join department in context.Departments.AsNoTracking()
                         on employee.DepartmentId equals department.DepartmentId into departments
                     from department in departments.DefaultIfEmpty()
                     select new
                     {
                         user.UserId,
                         user.PasswordHash,
                         user.LastDomainValidatedAt,
                         user.IsActive,
                         user.LockoutEndUtc,
                         DisplayNameTW = employee == null ? null : employee.DisplayNameTW,
                         DisplayNameVN = employee == null ? null : employee.DisplayNameVN,
                         DepartmentName = department == null ? null : department.DepartmentName,
                         DepartmentCode = department == null ? null : department.DepartmentCode
                     })
                    .SingleOrDefault();

                if (accountData == null)
                    return null;

                DateTime utcNow = DateTime.UtcNow;
                string[] roleNames =
                    (from userGroup in context.UserGroups.AsNoTracking()
                     join securityGroup in context.SecurityGroups.AsNoTracking()
                         on userGroup.GroupId equals securityGroup.GroupId
                     join groupRole in context.GroupRoles.AsNoTracking()
                         on securityGroup.GroupId equals groupRole.GroupId
                     join role in context.Roles.AsNoTracking()
                         on groupRole.RoleId equals role.RoleId
                     where userGroup.UserId == accountData.UserId
                           && userGroup.IsActive
                           && securityGroup.IsActive
                           && groupRole.IsActive
                           && role.IsActive
                           && (!userGroup.ExpiresAt.HasValue || userGroup.ExpiresAt > utcNow)
                     select role.RoleName)
                    .Distinct()
                    .ToArray();

                string[] permissionCodes =
                    (from userGroup in context.UserGroups.AsNoTracking()
                     join securityGroup in context.SecurityGroups.AsNoTracking()
                         on userGroup.GroupId equals securityGroup.GroupId
                     join groupRole in context.GroupRoles.AsNoTracking()
                         on securityGroup.GroupId equals groupRole.GroupId
                     join role in context.Roles.AsNoTracking()
                         on groupRole.RoleId equals role.RoleId
                     join rolePermission in context.RolePermissions.AsNoTracking()
                         on role.RoleId equals rolePermission.RoleId
                     join permission in context.Permissions.AsNoTracking()
                         on rolePermission.PermissionId equals permission.PermissionId
                     where userGroup.UserId == accountData.UserId
                           && userGroup.IsActive
                           && securityGroup.IsActive
                           && groupRole.IsActive
                           && role.IsActive
                           && rolePermission.IsActive
                           && permission.IsActive
                           && (!userGroup.ExpiresAt.HasValue || userGroup.ExpiresAt > utcNow)
                     select permission.PermissionCode)
                    .Distinct()
                    .ToArray();

                return new UserAccountRecord
                {
                    UserId = accountData.UserId,
                    CachedDomainPasswordHash = accountData.PasswordHash,
                    LastDomainValidatedAt = accountData.LastDomainValidatedAt,
                    IsActive = accountData.IsActive,
                    LockoutEndUtc = accountData.LockoutEndUtc.HasValue
                        ? DateTime.SpecifyKind(accountData.LockoutEndUtc.Value, DateTimeKind.Utc)
                        : (DateTime?)null,
                    DisplayNameTW = accountData.DisplayNameTW,
                    DisplayNameVN = accountData.DisplayNameVN,
                    Department = accountData.DepartmentName ?? string.Empty,
                    DepartmentCode = accountData.DepartmentCode ?? string.Empty,
                    Roles = string.Join("、", roleNames),
                    PermissionCodes = permissionCodes
                };
            }
        }

        public DateTime? RecordFailedLogin(string userId, int maximumAttempts, int lockoutMinutes)
        {
            using (var context = CreateContext())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = context.UserAccounts.SingleOrDefault(item => item.UserId == userId);

                DateTime? lockoutEndUtc = null;
                if (account != null)
                {
                    account.FailedLoginCount++;
                    if (account.FailedLoginCount >= maximumAttempts)
                    {
                        lockoutEndUtc = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                        account.LockoutEndUtc = lockoutEndUtc;
                    }

                    account.UpdatedAt = DateTime.UtcNow;
                }

                context.AuditLogs.Add(CreateAuditLog(
                    account?.UserId,
                    userId,
                    "AUTH.LOGIN.FAILED",
                    "登入失敗。"));
                if (lockoutEndUtc.HasValue)
                {
                    context.AuditLogs.Add(CreateAuditLog(
                        account.UserId,
                        userId,
                        "AUTH.ACCOUNT.LOCKED",
                        "登入失敗次數過多，帳號已暫時鎖定。"));
                }
                context.SaveChanges();
                transaction.Commit();
                return lockoutEndUtc;
            }
        }

        public void UpdateDomainCredentialCache(string userId, string passwordHash)
        {
            using (var context = CreateContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                UserAccount account = context.UserAccounts.SingleOrDefault(item => item.UserId == userId);
                if (account == null || !account.IsActive) return;
                account.PasswordHash = passwordHash;
                account.LastDomainValidatedAt = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAuditLog(account.UserId, account.UserId, "AUTH.DOMAIN.CACHE.REFRESHED", "已更新離線登入驗證資料。"));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public bool RecordSuccessfulLogin(string userId)
        {
            using (var context = CreateContext())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = context.UserAccounts.SingleOrDefault(item => item.UserId == userId);
                DateTime utcNow = DateTime.UtcNow;
                if (account == null
                    || !account.IsActive
                    || (account.LockoutEndUtc.HasValue && account.LockoutEndUtc.Value > utcNow))
                {
                    return false;
                }

                bool wasAutomaticallyUnlocked = account.LockoutEndUtc.HasValue;
                account.FailedLoginCount = 0;
                account.LockoutEndUtc = null;
                account.LastLoginAt = utcNow;
                account.UpdatedAt = utcNow;
                context.AuditLogs.Add(CreateAuditLog(
                    account.UserId,
                    userId,
                    "AUTH.LOGIN.SUCCEEDED",
                    "登入成功。"));
                if (wasAutomaticallyUnlocked)
                {
                    context.AuditLogs.Add(CreateAuditLog(
                        account.UserId,
                        userId,
                        "AUTH.ACCOUNT.UNLOCKED",
                        "鎖定期間已結束，帳號已自動解除鎖定。"));
                }
                context.SaveChanges();
                transaction.Commit();
                return true;
            }
        }

        private Winform4SystemDbContext CreateContext()
        {
            return new Winform4SystemDbContext(_connectionString);
        }

        private static AuditLog CreateAuditLog(string userId, string attemptedUserId, string actionCode, string description)
        {
            return new AuditLog
            {
                UserId = userId,
                AttemptedUserId = attemptedUserId,
                ActionCode = actionCode,
                Description = description,
                MachineName = Environment.MachineName
            };
        }
    }
}
