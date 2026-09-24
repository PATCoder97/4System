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
    public sealed class DepartmentManagementService
    {
        private static readonly Regex CodePattern = new Regex("^[A-Z0-9._-]{1,30}$", RegexOptions.Compiled);
        private readonly string _connectionString;

        public DepartmentManagementService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public DepartmentManagementService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));
            _connectionString = connectionString;
        }

        public IReadOnlyList<DepartmentListItem> GetDepartments()
        {
            CurrentAuthorization.Demand("SYSTEM.DEPARTMENT.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.Departments.AsNoTracking()
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.DepartmentCode)
                    .Select(x => new DepartmentListItem
                    {
                        DepartmentId = x.DepartmentId,
                        ParentDepartmentId = x.ParentDepartmentId,
                        DepartmentCode = x.DepartmentCode,
                        DepartmentName = x.DepartmentName,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive,
                        EmployeeCount = context.EmployeeProfiles.Count(employee => employee.DepartmentId == x.DepartmentId && employee.EmploymentStatus != 0 && employee.EmploymentStatus != 3),
                        ChildCount = context.Departments.Count(child => child.ParentDepartmentId == x.DepartmentId),
                        RowVersion = x.RowVersion
                    }).ToList();
            }
        }

        public int Save(DepartmentEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.DEPARTMENT.ADMIN");
            Validate(model);
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                Department department = model.DepartmentId.HasValue
                    ? context.Departments.FirstOrDefault(x => x.DepartmentId == model.DepartmentId.Value)
                    : null;
                bool isNew = department == null;
                if (model.DepartmentId.HasValue && department == null) throw new InvalidOperationException("找不到需要更新的部門。");
                if (department != null && !RowVersionMatches(department.RowVersion, model.RowVersion))
                    throw new InvalidOperationException("此部門已由其他使用者更新，請重新載入後再試。");

                string code = model.DepartmentCode.Trim().ToUpperInvariant();
                if (context.Departments.Any(x => x.DepartmentCode == code && (!model.DepartmentId.HasValue || x.DepartmentId != model.DepartmentId.Value)))
                    throw new InvalidOperationException("此部門代碼已存在。");
                if (!isNew && !string.Equals(department.DepartmentCode, code, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("建立後不可變更部門代碼。");

                ValidateParent(context, model.DepartmentId, model.ParentDepartmentId);
                if (!model.IsActive && model.DepartmentId.HasValue) EnsureCanDeactivate(context, model.DepartmentId.Value);

                Dictionary<string, string> before = isNew ? null : Snapshot(department);
                if (isNew)
                {
                    department = new Department { DepartmentCode = code };
                    context.Departments.Add(department);
                }
                department.DepartmentName = model.DepartmentName.Trim();
                department.ParentDepartmentId = model.ParentDepartmentId;
                department.SortOrder = model.SortOrder;
                department.IsActive = model.IsActive;
                department.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();
                context.AuditLogs.Add(CreateAudit(
                    department.DepartmentId,
                    isNew ? "DEPARTMENT.CREATE" : "DEPARTMENT.UPDATE",
                    isNew ? "已建立部門。" : "已更新部門。",
                    before,
                    Snapshot(department)));
                context.SaveChanges();
                transaction.Commit();
                return department.DepartmentId;
            }
        }

        public void SetActive(int departmentId, bool isActive, byte[] rowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.DEPARTMENT.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                Department department = context.Departments.FirstOrDefault(x => x.DepartmentId == departmentId);
                if (department == null) throw new InvalidOperationException("找不到所選部門。");
                if (!RowVersionMatches(department.RowVersion, rowVersion)) throw new InvalidOperationException("此部門已由其他使用者更新，請重新載入後再試。");
                if (department.IsActive == isActive) return;
                if (!isActive) EnsureCanDeactivate(context, departmentId);
                Dictionary<string, string> before = Snapshot(department);
                department.IsActive = isActive;
                department.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAudit(departmentId, isActive ? "DEPARTMENT.ENABLE" : "DEPARTMENT.DISABLE", isActive ? "已啟用部門。" : "已停用部門。", before, Snapshot(department)));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public IReadOnlyList<EmployeeDepartmentTransferItem> GetTransferCandidates(int sourceDepartmentId)
        {
            CurrentAuthorization.Demand("SYSTEM.DEPARTMENT.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.EmployeeProfiles.AsNoTracking()
                    .Where(x => x.DepartmentId == sourceDepartmentId && x.EmploymentStatus != 0 && x.EmploymentStatus != 3)
                    .OrderBy(x => x.EmployeeCode)
                    .Select(x => new EmployeeDepartmentTransferItem
                    {
                        EmployeeProfileId = x.EmployeeProfileId,
                        UserId = x.EmployeeCode,
                        DisplayNameTW = x.DisplayNameTW,
                        DisplayNameVN = x.DisplayNameVN,
                        EmploymentStatus = x.EmploymentStatus,
                        RowVersion = x.RowVersion
                    }).ToList();
            }
        }

        public int TransferEmployees(int sourceDepartmentId, int targetDepartmentId, IEnumerable<EmployeeDepartmentTransferItem> previewedEmployees)
        {
            CurrentAuthorization.Demand("SYSTEM.DEPARTMENT.ADMIN");
            if (sourceDepartmentId == targetDepartmentId) throw new InvalidOperationException("來源與目標部門不可相同。");
            List<EmployeeDepartmentTransferItem> requests = (previewedEmployees ?? Enumerable.Empty<EmployeeDepartmentTransferItem>())
                .GroupBy(x => x.EmployeeProfileId).Select(x => x.First()).ToList();
            if (requests.Count == 0) throw new InvalidOperationException("來源部門沒有可轉移的有效人員。");

            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                Department source = context.Departments.FirstOrDefault(x => x.DepartmentId == sourceDepartmentId);
                Department target = context.Departments.FirstOrDefault(x => x.DepartmentId == targetDepartmentId);
                if (source == null) throw new InvalidOperationException("找不到來源部門。");
                if (target == null || !target.IsActive) throw new InvalidOperationException("目標部門不存在或已停用。");

                long[] employeeIds = requests.Select(x => x.EmployeeProfileId).ToArray();
                List<EmployeeProfile> employees = context.EmployeeProfiles.Where(x => employeeIds.Contains(x.EmployeeProfileId)).ToList();
                if (employees.Count != requests.Count) throw new InvalidOperationException("部分人員資料已不存在，請重新載入後再試。");

                foreach (EmployeeProfile employee in employees)
                {
                    EmployeeDepartmentTransferItem request = requests.First(x => x.EmployeeProfileId == employee.EmployeeProfileId);
                    if (employee.DepartmentId != sourceDepartmentId || employee.EmploymentStatus == 0 || employee.EmploymentStatus == 3)
                        throw new InvalidOperationException("人員資料或任職狀態已變更，請重新載入後再試。");
                    if (!RowVersionMatches(employee.RowVersion, request.RowVersion))
                        throw new InvalidOperationException("人員資料已由其他使用者更新，請重新載入後再試。");

                    int previousDepartmentId = employee.DepartmentId;
                    employee.DepartmentId = targetDepartmentId;
                    employee.UpdatedAt = DateTime.UtcNow;
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = CurrentAuthorization.UserId,
                        AttemptedUserId = employee.EmployeeCode,
                        ActionCode = "EMPLOYEE.DEPARTMENT.TRANSFER",
                        EntityName = "hr_EmployeeProfile",
                        EntityId = employee.EmployeeCode,
                        Description = "已將人員從「" + source.DepartmentName + "」轉移至「" + target.DepartmentName + "」。",
                        MachineName = Environment.MachineName,
                        DataJson = AuditDataJson.Change(
                            new Dictionary<string, string> { { "departmentId", previousDepartmentId.ToString() } },
                            new Dictionary<string, string> { { "departmentId", targetDepartmentId.ToString() } })
                    });
                }

                context.SaveChanges();
                transaction.Commit();
                return employees.Count;
            }
        }

        private static void Validate(DepartmentEditModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            string code = (model.DepartmentCode ?? string.Empty).Trim().ToUpperInvariant();
            if (!CodePattern.IsMatch(code)) throw new InvalidOperationException("部門代碼只能包含大寫英文字母、數字、句點、底線或連字號，最長 30 個字元。");
            if (string.IsNullOrWhiteSpace(model.DepartmentName)) throw new InvalidOperationException("請輸入部門名稱。");
            if (model.DepartmentName.Trim().Length > 200) throw new InvalidOperationException("部門名稱不可超過 200 個字元。");
            if (model.SortOrder < 0) throw new InvalidOperationException("排序不可小於 0。");
        }

        private static void ValidateParent(Winform4SystemDbContext context, int? departmentId, int? parentId)
        {
            if (!parentId.HasValue) return;
            if (departmentId == parentId) throw new InvalidOperationException("部門不可將自己設為上層部門。");
            Department parent = context.Departments.AsNoTracking().FirstOrDefault(x => x.DepartmentId == parentId.Value);
            if (parent == null || !parent.IsActive) throw new InvalidOperationException("上層部門不存在或已停用。");
            if (!departmentId.HasValue) return;

            int? currentId = parent.ParentDepartmentId;
            while (currentId.HasValue)
            {
                if (currentId.Value == departmentId.Value) throw new InvalidOperationException("不可將部門移到自己的下層部門之下。");
                currentId = context.Departments.AsNoTracking().Where(x => x.DepartmentId == currentId.Value).Select(x => x.ParentDepartmentId).FirstOrDefault();
            }
        }

        private static void EnsureCanDeactivate(Winform4SystemDbContext context, int departmentId)
        {
            bool hasActiveEmployees = context.EmployeeProfiles.Any(x => x.DepartmentId == departmentId && x.EmploymentStatus != 0 && x.EmploymentStatus != 3);
            if (hasActiveEmployees) throw new InvalidOperationException("此部門仍有在職或留職停薪人員，請先轉移人員後再停用。");
            bool hasActiveChildren = context.Departments.Any(x => x.ParentDepartmentId == departmentId && x.IsActive);
            if (hasActiveChildren) throw new InvalidOperationException("此部門仍有啟用中的下層部門，請先調整或停用下層部門。");
        }

        private static bool RowVersionMatches(byte[] current, byte[] original)
        {
            return current != null && original != null && current.SequenceEqual(original);
        }

        private static Dictionary<string, string> Snapshot(Department department)
        {
            return new Dictionary<string, string>
            {
                { "departmentCode", department.DepartmentCode },
                { "departmentName", department.DepartmentName },
                { "parentDepartmentId", department.ParentDepartmentId?.ToString() },
                { "sortOrder", department.SortOrder.ToString() },
                { "isActive", department.IsActive.ToString() }
            };
        }

        private static AuditLog CreateAudit(int departmentId, string actionCode, string description, IDictionary<string, string> before, IDictionary<string, string> after)
        {
            return new AuditLog
            {
                UserId = CurrentAuthorization.UserId,
                ActionCode = actionCode,
                EntityName = "dm_Department",
                EntityId = departmentId.ToString(),
                Description = description,
                MachineName = Environment.MachineName,
                DataJson = AuditDataJson.Change(before, after)
            };
        }
    }

    public sealed class DepartmentListItem
    {
        public int DepartmentId { get; set; }
        public int? ParentDepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeCount { get; set; }
        public int ChildCount { get; set; }
        public byte[] RowVersion { get; set; }
        public string StatusDisplay => IsActive ? "啟用" : "停用";
        public string DisplayName => DepartmentName + "（" + DepartmentCode + "）";
    }

    public sealed class DepartmentEditModel
    {
        public int? DepartmentId { get; set; }
        public int? ParentDepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
    }

    public sealed class EmployeeDepartmentTransferItem
    {
        public long EmployeeProfileId { get; set; }
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public byte EmploymentStatus { get; set; }
        public byte[] RowVersion { get; set; }
        public string EmploymentStatusDisplay => EmploymentStatus == 1 ? "在職" : EmploymentStatus == 2 ? "留職停薪" : "其他";
    }
}
