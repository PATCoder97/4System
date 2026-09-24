using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Winform4System.Business.Security;
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
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();

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
                            AuthenticationType = account == null ? null : account.AuthenticationType,
                            DomainAccount = account == null ? null : account.DomainAccount,
                            IsAccountActive = account != null && account.IsActive,
                            HasAccount = account != null
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
            using (var transaction = context.Database.BeginTransaction())
            {
                var employee = model.EmployeeProfileId.HasValue
                    ? context.EmployeeProfiles.FirstOrDefault(x => x.EmployeeProfileId == model.EmployeeProfileId.Value)
                    : null;
                bool isNew = employee == null;
                var existingAccount = context.UserAccounts.FirstOrDefault(x => x.UserId == userId);
                var before = isNew ? null : CreateAuditSnapshot(employee, existingAccount);
                if (model.EmployeeProfileId.HasValue && employee == null)
                    throw new InvalidOperationException("找不到需要更新的人員資料。");
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
                account.AuthenticationType = model.AuthenticationType;
                account.DomainAccount = model.AuthenticationType == "WINDOWS" ? Normalize(model.DomainAccount) : null;
                account.IsActive = model.IsAccountActive;
                account.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(model.NewPassword)) account.PasswordHash = _passwordHasher.Hash(model.NewPassword);
                if (model.AuthenticationType == "LOCAL" && string.IsNullOrWhiteSpace(account.PasswordHash))
                    throw new InvalidOperationException("本機帳號必須設定密碼。");

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

        public void Deactivate(string userId)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                var account = context.UserAccounts.FirstOrDefault(x => x.UserId == userId);
                var employee = context.EmployeeProfiles.FirstOrDefault(x => x.EmployeeCode == userId);
                if (employee == null) throw new InvalidOperationException("找不到所選人員。");
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
            if (model.AuthenticationType != "LOCAL" && model.AuthenticationType != "WINDOWS") throw new InvalidOperationException("驗證方式無效。");
            if (model.AuthenticationType == "WINDOWS" && string.IsNullOrWhiteSpace(model.DomainAccount)) throw new InvalidOperationException("Windows 驗證必須輸入網域帳號。");
            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword.Length < 8) throw new InvalidOperationException("密碼至少需要 8 個字元。");
        }

        private static string Normalize(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static Dictionary<string, string> CreateAuditSnapshot(EmployeeProfile employee, UserAccount account)
        {
            if (employee == null) return null;
            return new Dictionary<string, string>
            {
                { "displayNameTW", employee.DisplayNameTW },
                { "displayNameVN", employee.DisplayNameVN },
                { "departmentId", employee.DepartmentId.ToString() },
                { "employmentStatus", employee.EmploymentStatus.ToString() },
                { "authenticationType", account?.AuthenticationType },
                { "domainAccount", account?.DomainAccount },
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
        public string AuthenticationType { get; set; }
        public string AuthenticationTypeDisplay => AuthenticationType == "WINDOWS" ? "Windows 網域" : AuthenticationType == "LOCAL" ? "本機密碼" : "未建立";
        public string DomainAccount { get; set; }
        public bool IsAccountActive { get; set; }
        public bool HasAccount { get; set; }
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
        public string AuthenticationType { get; set; }
        public string DomainAccount { get; set; }
        public string NewPassword { get; set; }
        public bool IsAccountActive { get; set; }
    }

    public sealed class DepartmentOption
    {
        public int DepartmentId { get; set; }
        public string DisplayName { get; set; }
    }
}
