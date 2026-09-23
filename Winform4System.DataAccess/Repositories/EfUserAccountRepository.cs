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

        public UserAccountRecord FindByLoginName(string normalizedLoginName)
        {
            using (var context = CreateContext())
            {
                var accountData =
                    (from user in context.UserAccounts.AsNoTracking()
                     where user.NormalizedLoginName == normalizedLoginName
                     join employee in context.EmployeeProfiles.AsNoTracking()
                         on user.EmployeeProfileId equals employee.EmployeeProfileId into employees
                     from employee in employees.DefaultIfEmpty()
                     join department in context.Departments.AsNoTracking()
                         on employee.DepartmentId equals department.DepartmentId into departments
                     from department in departments.DefaultIfEmpty()
                     select new
                     {
                         user.UserId,
                         user.LoginName,
                         user.AuthenticationType,
                         user.PasswordHash,
                         user.IsActive,
                         user.LockoutEndUtc,
                         FullName = employee == null ? null : employee.FullName,
                         PreferredName = employee == null ? null : employee.PreferredName,
                         DepartmentName = department == null ? null : department.DepartmentName
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

                string displayName = !string.IsNullOrWhiteSpace(accountData.PreferredName)
                    ? accountData.PreferredName
                    : !string.IsNullOrWhiteSpace(accountData.FullName)
                        ? accountData.FullName
                        : accountData.LoginName;

                return new UserAccountRecord
                {
                    UserId = accountData.UserId,
                    LoginName = accountData.LoginName,
                    AuthenticationType = accountData.AuthenticationType,
                    PasswordHash = accountData.PasswordHash,
                    IsActive = accountData.IsActive,
                    LockoutEndUtc = accountData.LockoutEndUtc.HasValue
                        ? DateTime.SpecifyKind(accountData.LockoutEndUtc.Value, DateTimeKind.Utc)
                        : (DateTime?)null,
                    DisplayName = displayName,
                    Department = accountData.DepartmentName ?? string.Empty,
                    Roles = string.Join("、", roleNames)
                };
            }
        }

        public DateTime? RecordFailedLogin(long? userId, string loginName, int maximumAttempts, int lockoutMinutes)
        {
            using (var context = CreateContext())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = userId.HasValue
                    ? context.UserAccounts.SingleOrDefault(item => item.UserId == userId.Value)
                    : null;

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
                    loginName,
                    "AUTH.LOGIN.FAILED",
                    "登入失敗。"));
                context.SaveChanges();
                transaction.Commit();
                return lockoutEndUtc;
            }
        }

        public bool RecordSuccessfulLogin(long userId, string loginName)
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

                account.FailedLoginCount = 0;
                account.LockoutEndUtc = null;
                account.LastLoginAt = utcNow;
                account.UpdatedAt = utcNow;
                context.AuditLogs.Add(CreateAuditLog(
                    account.UserId,
                    loginName,
                    "AUTH.LOGIN.SUCCEEDED",
                    "登入成功。"));
                context.SaveChanges();
                transaction.Commit();
                return true;
            }
        }

        private Winform4SystemDbContext CreateContext()
        {
            return new Winform4SystemDbContext(_connectionString);
        }

        private static AuditLog CreateAuditLog(long? userId, string loginName, string actionCode, string description)
        {
            return new AuditLog
            {
                UserId = userId,
                LoginName = loginName,
                ActionCode = actionCode,
                Description = description,
                MachineName = Environment.MachineName
            };
        }
    }
}
