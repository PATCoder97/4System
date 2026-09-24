using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class EmployeeManagementService
    {
        private static readonly Regex UserIdPattern = new Regex("^VNW[0-9]{7}$", RegexOptions.Compiled);
        private readonly string _connectionString;

        public EmployeeManagementService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public IReadOnlyList<EmployeeListItem> GetEmployees()
        {
            CurrentAuthorization.Demand("SYSTEM.USER.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return (from employee in context.EmployeeProfiles.AsNoTracking()
                        join department in context.Departments.AsNoTracking() on employee.DepartmentId equals department.DepartmentId
                        join jobTitle in context.JobTitles.AsNoTracking() on employee.JobTitleId equals (int?)jobTitle.JobTitleId into jobTitles
                        from jobTitle in jobTitles.DefaultIfEmpty()
                        join account in context.UserAccounts.AsNoTracking() on employee.EmployeeProfileId equals account.EmployeeProfileId into accounts
                        from account in accounts.DefaultIfEmpty()
                        orderby employee.EmployeeCode
                        select new EmployeeListItem
                        {
                            EmployeeProfileId = employee.EmployeeProfileId,
                            UserId = employee.EmployeeCode,
                            DisplayNameTW = employee.DisplayNameTW,
                            DisplayNameVN = employee.DisplayNameVN,
                            DepartmentId = employee.DepartmentId,
                            DepartmentName = department.DepartmentName,
                            JobTitleId = employee.JobTitleId,
                            JobTitleName = jobTitle == null ? null : jobTitle.JobTitleName,
                            WorkEmail = employee.WorkEmail,
                            WorkPhone = employee.WorkPhone,
                            HireDate = employee.HireDate,
                            EmploymentStatus = employee.EmploymentStatus,
                            IsAccountActive = account != null && account.IsActive,
                            HasAccount = account != null,
                            FailedLoginCount = account == null ? 0 : account.FailedLoginCount,
                            LockoutEndUtc = account == null ? null : account.LockoutEndUtc,
                            LastLoginAt = account == null ? null : account.LastLoginAt,
                            EmployeeRowVersion = employee.RowVersion,
                            AccountRowVersion = account == null ? null : account.RowVersion
                        }).ToList();
            }
        }

        public IReadOnlyList<DepartmentOption> GetDepartments()
        {
            CurrentAuthorization.Demand("SYSTEM.USER.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.Departments.AsNoTracking().Where(x => x.IsActive)
                    .OrderBy(x => x.DepartmentCode)
                    .Select(x => new DepartmentOption { DepartmentId = x.DepartmentId, DisplayName = x.DepartmentCode + " - " + x.DepartmentName })
                    .ToList();
            }
        }

        public IReadOnlyList<JobTitleOption> GetJobTitles()
        {
            CurrentAuthorization.Demand("SYSTEM.USER.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.JobTitles.AsNoTracking().Where(x => x.IsActive)
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.JobTitleCode)
                    .Select(x => new JobTitleOption { JobTitleId = x.JobTitleId, DisplayName = x.JobTitleCode + " - " + x.JobTitleName })
                    .ToList();
            }
        }

        public void Save(EmployeeEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            Validate(model);
            string userId = model.UserId.Trim().ToUpperInvariant();
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var employee = model.EmployeeProfileId.HasValue
                    ? context.EmployeeProfiles.FirstOrDefault(x => x.EmployeeProfileId == model.EmployeeProfileId.Value)
                    : null;
                bool isNew = employee == null;
                var existingAccount = context.UserAccounts.FirstOrDefault(x => x.UserId == userId);
                var before = isNew ? null : CreateAuditSnapshot(employee, existingAccount);
                if (model.EmployeeProfileId.HasValue && employee == null)
                    throw new InvalidOperationException("找不到需要更新的人員資料。");
                if (employee != null && !RowVersionMatches(employee.RowVersion, model.EmployeeRowVersion))
                    throw new InvalidOperationException("此人員資料已由其他使用者更新，請重新載入後再試。");
                if (existingAccount != null && !RowVersionMatches(existingAccount.RowVersion, model.AccountRowVersion))
                    throw new InvalidOperationException("此登入帳號已由其他使用者更新，請重新載入後再試。");
                if (employee == null)
                {
                    if (context.EmployeeProfiles.Any(x => x.EmployeeCode == userId))
                        throw new InvalidOperationException("此人員編號已存在。");
                    employee = new EmployeeProfile { EmployeeCode = userId };
                    context.EmployeeProfiles.Add(employee);
                }
                else if (!string.Equals(employee.EmployeeCode, userId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("建立後不可變更人員編號。");
                }

                employee.DisplayNameTW = Normalize(model.DisplayNameTW);
                employee.DisplayNameVN = model.DisplayNameVN.Trim();
                employee.DepartmentId = model.DepartmentId;
                employee.JobTitleId = model.JobTitleId;
                employee.WorkEmail = Normalize(model.WorkEmail);
                employee.WorkPhone = Normalize(model.WorkPhone);
                employee.HireDate = model.HireDate;
                employee.EmploymentStatus = model.EmploymentStatus;
                employee.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();

                var account = existingAccount;
                if (account == null)
                {
                    account = new UserAccount
                    {
                        UserId = userId,
                        FailedLoginCount = 0,
                        SecurityStamp = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow
                    };
                    context.UserAccounts.Add(account);
                }
                account.EmployeeProfileId = employee.EmployeeProfileId;
                if (!model.IsAccountActive) EnsureCanDisableAccount(context, userId);
                if (account.IsActive != model.IsAccountActive) account.SecurityStamp = Guid.NewGuid();
                account.IsActive = model.IsAccountActive;
                account.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = isNew ? "EMPLOYEE.CREATE" : "EMPLOYEE.UPDATE",
                    EntityName = "hr_EmployeeProfile",
                    EntityId = userId,
                    Description = (isNew ? "建立" : "更新") + "人員與帳號 " + userId,
                    MachineName = Environment.MachineName,
                    DataJson = AuditDataJson.Change(before, CreateAuditSnapshot(employee, account))
                });
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void Deactivate(string userId, byte[] employeeRowVersion, byte[] accountRowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var account = context.UserAccounts.FirstOrDefault(x => x.UserId == userId);
                var employee = context.EmployeeProfiles.FirstOrDefault(x => x.EmployeeCode == userId);
                if (employee == null) throw new InvalidOperationException("找不到所選人員。");
                if (!RowVersionMatches(employee.RowVersion, employeeRowVersion)) throw new InvalidOperationException("此人員資料已由其他使用者更新，請重新載入後再試。");
                if (account != null && !RowVersionMatches(account.RowVersion, accountRowVersion)) throw new InvalidOperationException("此登入帳號已由其他使用者更新，請重新載入後再試。");
                EnsureCanDisableAccount(context, userId);
                byte previousEmploymentStatus = employee.EmploymentStatus;
                bool? previousAccountStatus = account?.IsActive;
                employee.EmploymentStatus = 0;
                employee.ResignDate = employee.ResignDate ?? DateTime.Today;
                employee.UpdatedAt = DateTime.UtcNow;
                if (account != null) { account.IsActive = false; account.SecurityStamp = Guid.NewGuid(); account.UpdatedAt = DateTime.UtcNow; }
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = "EMPLOYEE.DEACTIVATE",
                    EntityName = "hr_EmployeeProfile",
                    EntityId = userId,
                    Description = "停用人員與帳號 " + userId,
                    MachineName = Environment.MachineName,
                    DataJson = AuditDataJson.Change(
                        new Dictionary<string, string> { { "employmentStatus", previousEmploymentStatus.ToString() }, { "accountActive", previousAccountStatus?.ToString() } },
                        new Dictionary<string, string> { { "employmentStatus", "0" }, { "accountActive", account == null ? null : "False" } })
                });
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void UnlockAccount(string userId, byte[] accountRowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = GetAccountForSecurityAction(context, userId, accountRowVersion);
                int previousFailedCount = account.FailedLoginCount;
                DateTime? previousLockoutEnd = account.LockoutEndUtc;
                account.FailedLoginCount = 0;
                account.LockoutEndUtc = null;
                account.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAccountAudit(
                    account.UserId,
                    "AUTH.ACCOUNT.UNLOCKED.BY_ADMIN",
                    "管理員已解除帳號鎖定。",
                    new Dictionary<string, string> { { "failedLoginCount", previousFailedCount.ToString() }, { "lockoutEndUtc", previousLockoutEnd?.ToString("O") } },
                    new Dictionary<string, string> { { "failedLoginCount", "0" }, { "lockoutEndUtc", null } }));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void SetAccountActive(string userId, bool isActive, byte[] accountRowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = GetAccountForSecurityAction(context, userId, accountRowVersion);
                if (account.IsActive == isActive) return;
                if (!isActive) EnsureCanDisableAccount(context, account.UserId);

                bool previousStatus = account.IsActive;
                account.IsActive = isActive;
                account.SecurityStamp = Guid.NewGuid();
                account.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAccountAudit(
                    account.UserId,
                    isActive ? "AUTH.ACCOUNT.ENABLED" : "AUTH.ACCOUNT.DISABLED",
                    isActive ? "管理員已啟用登入帳號。" : "管理員已停用登入帳號。",
                    new Dictionary<string, string> { { "isActive", previousStatus.ToString() } },
                    new Dictionary<string, string> { { "isActive", isActive.ToString() } }));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void RevokeSessions(string userId, byte[] accountRowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                UserAccount account = GetAccountForSecurityAction(context, userId, accountRowVersion);
                account.SecurityStamp = Guid.NewGuid();
                account.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAccountAudit(account.UserId, "AUTH.SESSION.REVOKED", "管理員已撤銷此帳號的登入工作階段。", null, null));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void Validate(EmployeeEditModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            model.UserId = (model.UserId ?? string.Empty).Trim().ToUpperInvariant();
            if (!UserIdPattern.IsMatch(model.UserId)) throw new InvalidOperationException("人員編號必須為 VNW 加上 7 位數字。");
            if (string.IsNullOrWhiteSpace(model.DisplayNameVN)) throw new InvalidOperationException("請輸入越文姓名。");
            if (model.DepartmentId <= 0) throw new InvalidOperationException("請選擇部門。");
            if (model.EmploymentStatus > 3) throw new InvalidOperationException("任職狀態無效。");
        }

        private static string Normalize(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static bool RowVersionMatches(byte[] current, byte[] original)
        {
            return current != null && original != null && current.SequenceEqual(original);
        }

        private static void EnsureCanDisableAccount(Winform4SystemDbContext context, string userId)
        {
            if (string.Equals(userId, CurrentAuthorization.UserId, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("不可停用目前登入的帳號。請由其他管理員執行此操作。");

            int? administratorGroupId = context.SecurityGroups
                .Where(x => x.GroupCode == "SYSTEM_ADMINISTRATORS" && x.IsActive)
                .Select(x => (int?)x.GroupId).FirstOrDefault();
            if (!administratorGroupId.HasValue) return;
            bool isAdministrator = context.UserGroups.Any(x => x.UserId == userId && x.GroupId == administratorGroupId.Value && x.IsActive && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow));
            if (!isAdministrator) return;
            bool hasAnotherAdministrator = context.UserGroups.Any(x => x.UserId != userId && x.GroupId == administratorGroupId.Value && x.IsActive && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow)
                && context.UserAccounts.Any(account => account.UserId == x.UserId && account.IsActive));
            if (!hasAnotherAdministrator)
                throw new InvalidOperationException("系統必須保留至少一個有效的管理員帳號，無法停用最後一位管理員。");
        }

        private static UserAccount GetAccountForSecurityAction(Winform4SystemDbContext context, string userId, byte[] accountRowVersion)
        {
            UserAccount account = context.UserAccounts.FirstOrDefault(x => x.UserId == userId);
            if (account == null) throw new InvalidOperationException("找不到所選登入帳號。");
            if (!RowVersionMatches(account.RowVersion, accountRowVersion))
                throw new InvalidOperationException("此登入帳號已由其他使用者更新，請重新載入後再試。");
            return account;
        }

        private static AuditLog CreateAccountAudit(string targetUserId, string actionCode, string description, IDictionary<string, string> before, IDictionary<string, string> after)
        {
            return new AuditLog
            {
                UserId = CurrentAuthorization.UserId,
                AttemptedUserId = targetUserId,
                ActionCode = actionCode,
                EntityName = "auth_UserAccount",
                EntityId = targetUserId,
                Description = description + " " + targetUserId,
                MachineName = Environment.MachineName,
                DataJson = before == null && after == null ? null : AuditDataJson.Change(before, after)
            };
        }

        private static Dictionary<string, string> CreateAuditSnapshot(EmployeeProfile employee, UserAccount account)
        {
            if (employee == null) return null;
            return new Dictionary<string, string>
            {
                { "displayNameTW", employee.DisplayNameTW },
                { "displayNameVN", employee.DisplayNameVN },
                { "departmentId", employee.DepartmentId.ToString() },
                { "jobTitleId", employee.JobTitleId?.ToString() },
                { "employmentStatus", employee.EmploymentStatus.ToString() },
                { "accountActive", account == null ? null : account.IsActive.ToString() }
            };
        }
    }

    public sealed class EmployeeListItem
    {
        public long EmployeeProfileId { get; set; }
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? JobTitleId { get; set; }
        public string JobTitleName { get; set; }
        public string WorkEmail { get; set; }
        public string WorkPhone { get; set; }
        public DateTime? HireDate { get; set; }
        public byte EmploymentStatus { get; set; }
        public bool IsAccountActive { get; set; }
        public bool HasAccount { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public byte[] EmployeeRowVersion { get; set; }
        public byte[] AccountRowVersion { get; set; }
        public string EmploymentStatusDisplay
        {
            get
            {
                switch (EmploymentStatus)
                {
                    case 0: return "停職";
                    case 1: return "在職";
                    case 2: return "留職停薪";
                    case 3: return "離職";
                    default: return "未知";
                }
            }
        }
        public bool IsLocked => LockoutEndUtc.HasValue && LockoutEndUtc.Value > DateTime.UtcNow;
        public string AccountStatusDisplay => !HasAccount ? "未建立" : !IsAccountActive ? "已停用" : IsLocked ? "已鎖定" : "正常";
        public string LockoutEndDisplay => ToLocalDisplay(LockoutEndUtc);
        public string LastLoginDisplay => ToLocalDisplay(LastLoginAt);

        private static string ToLocalDisplay(DateTime? value)
        {
            if (!value.HasValue) return string.Empty;
            return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        }
    }

    public sealed class EmployeeEditModel
    {
        public long? EmployeeProfileId { get; set; }
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public int DepartmentId { get; set; }
        public int? JobTitleId { get; set; }
        public string WorkEmail { get; set; }
        public string WorkPhone { get; set; }
        public DateTime? HireDate { get; set; }
        public byte EmploymentStatus { get; set; }
        public bool IsAccountActive { get; set; }
        public byte[] EmployeeRowVersion { get; set; }
        public byte[] AccountRowVersion { get; set; }
    }

    public sealed class DepartmentOption
    {
        public int DepartmentId { get; set; }
        public string DisplayName { get; set; }
    }

    public sealed class JobTitleOption
    {
        public int JobTitleId { get; set; }
        public string DisplayName { get; set; }
    }
}
