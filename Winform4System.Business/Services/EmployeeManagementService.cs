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
                            WorkEmail = employee.WorkEmail,
                            WorkPhone = employee.WorkPhone,
                            HireDate = employee.HireDate,
                            EmploymentStatus = employee.EmploymentStatus,
                            IsAccountActive = account != null && account.IsActive,
                            HasAccount = account != null,
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
                employee.WorkEmail = Normalize(model.WorkEmail);
                employee.WorkPhone = Normalize(model.WorkPhone);
                employee.HireDate = model.HireDate;
                employee.EmploymentStatus = model.EmploymentStatus;
                employee.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();

                var account = existingAccount;
                if (account == null)
                {
                    account = new UserAccount { UserId = userId, FailedLoginCount = 0 };
                    context.UserAccounts.Add(account);
                }
                account.EmployeeProfileId = employee.EmployeeProfileId;
                account.AuthenticationType = "WINDOWS";
                account.DomainAccount = userId;
                if (!model.IsAccountActive) EnsureCanDisableAccount(context, userId);
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
                if (account != null) { account.IsActive = false; account.UpdatedAt = DateTime.UtcNow; }
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

        private static Dictionary<string, string> CreateAuditSnapshot(EmployeeProfile employee, UserAccount account)
        {
            if (employee == null) return null;
            return new Dictionary<string, string>
            {
                { "displayNameTW", employee.DisplayNameTW },
                { "displayNameVN", employee.DisplayNameVN },
                { "departmentId", employee.DepartmentId.ToString() },
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
        public string WorkEmail { get; set; }
        public string WorkPhone { get; set; }
        public DateTime? HireDate { get; set; }
        public byte EmploymentStatus { get; set; }
        public bool IsAccountActive { get; set; }
        public bool HasAccount { get; set; }
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
    }

    public sealed class EmployeeEditModel
    {
        public long? EmployeeProfileId { get; set; }
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public int DepartmentId { get; set; }
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
}
