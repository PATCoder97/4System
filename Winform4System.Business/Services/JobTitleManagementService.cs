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
    public sealed class JobTitleManagementService
    {
        private static readonly Regex CodePattern = new Regex("^[A-Z0-9._-]{1,30}$", RegexOptions.Compiled);
        private readonly string _connectionString;

        public JobTitleManagementService(ConnectionStringProvider provider) { _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get(); }
        public JobTitleManagementService(string connectionString) { if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString)); _connectionString = connectionString; }

        public IReadOnlyList<JobTitleListItem> GetJobTitles()
        {
            CurrentAuthorization.Demand("SYSTEM.JOB_TITLE.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.JobTitles.AsNoTracking().OrderBy(x => x.SortOrder).ThenBy(x => x.JobTitleCode)
                    .Select(x => new JobTitleListItem
                    {
                        JobTitleId = x.JobTitleId,
                        JobTitleCode = x.JobTitleCode,
                        JobTitleName = x.JobTitleName,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive,
                        EmployeeCount = context.EmployeeProfiles.Count(employee => employee.JobTitleId == x.JobTitleId && employee.EmploymentStatus != 0 && employee.EmploymentStatus != 3),
                        RowVersion = x.RowVersion
                    }).ToList();
            }
        }

        public int Save(JobTitleEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.JOB_TITLE.ADMIN");
            Validate(model);
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                JobTitle item = model.JobTitleId.HasValue ? context.JobTitles.FirstOrDefault(x => x.JobTitleId == model.JobTitleId.Value) : null;
                bool isNew = item == null;
                if (model.JobTitleId.HasValue && item == null) throw new InvalidOperationException("找不到需要更新的職稱。");
                if (item != null && !RowVersionMatches(item.RowVersion, model.RowVersion)) throw new InvalidOperationException("此職稱已由其他使用者更新，請重新載入後再試。");
                string code = model.JobTitleCode.Trim().ToUpperInvariant();
                if (context.JobTitles.Any(x => x.JobTitleCode == code && (!model.JobTitleId.HasValue || x.JobTitleId != model.JobTitleId.Value))) throw new InvalidOperationException("此職稱代碼已存在。");
                if (!isNew && !string.Equals(item.JobTitleCode, code, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("建立後不可變更職稱代碼。");
                if (!model.IsActive && model.JobTitleId.HasValue) EnsureCanDeactivate(context, model.JobTitleId.Value);
                Dictionary<string, string> before = isNew ? null : Snapshot(item);
                if (isNew) { item = new JobTitle { JobTitleCode = code }; context.JobTitles.Add(item); }
                item.JobTitleName = model.JobTitleName.Trim();
                item.SortOrder = model.SortOrder;
                item.IsActive = model.IsActive;
                item.UpdatedAt = DateTime.UtcNow;
                context.SaveChanges();
                context.AuditLogs.Add(CreateAudit(item.JobTitleId, isNew ? "JOB_TITLE.CREATE" : "JOB_TITLE.UPDATE", isNew ? "已建立職稱。" : "已更新職稱。", before, Snapshot(item)));
                context.SaveChanges();
                transaction.Commit();
                return item.JobTitleId;
            }
        }

        public void SetActive(int jobTitleId, bool isActive, byte[] rowVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.JOB_TITLE.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                JobTitle item = context.JobTitles.FirstOrDefault(x => x.JobTitleId == jobTitleId);
                if (item == null) throw new InvalidOperationException("找不到所選職稱。");
                if (!RowVersionMatches(item.RowVersion, rowVersion)) throw new InvalidOperationException("此職稱已由其他使用者更新，請重新載入後再試。");
                if (item.IsActive == isActive) return;
                if (!isActive) EnsureCanDeactivate(context, jobTitleId);
                Dictionary<string, string> before = Snapshot(item);
                item.IsActive = isActive;
                item.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(CreateAudit(jobTitleId, isActive ? "JOB_TITLE.ENABLE" : "JOB_TITLE.DISABLE", isActive ? "已啟用職稱。" : "已停用職稱。", before, Snapshot(item)));
                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void Validate(JobTitleEditModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!CodePattern.IsMatch((model.JobTitleCode ?? string.Empty).Trim().ToUpperInvariant())) throw new InvalidOperationException("職稱代碼只能包含大寫英文字母、數字、句點、底線或連字號，最長 30 個字元。");
            if (string.IsNullOrWhiteSpace(model.JobTitleName)) throw new InvalidOperationException("請輸入職稱名稱。");
            if (model.JobTitleName.Trim().Length > 200) throw new InvalidOperationException("職稱名稱不可超過 200 個字元。");
            if (model.SortOrder < 0) throw new InvalidOperationException("排序不可小於 0。");
        }

        private static void EnsureCanDeactivate(Winform4SystemDbContext context, int jobTitleId)
        {
            if (context.EmployeeProfiles.Any(x => x.JobTitleId == jobTitleId && x.EmploymentStatus != 0 && x.EmploymentStatus != 3))
                throw new InvalidOperationException("此職稱仍有在職或留職停薪人員，請先調整人員職稱後再停用。");
        }

        private static bool RowVersionMatches(byte[] current, byte[] original) { return current != null && original != null && current.SequenceEqual(original); }
        private static Dictionary<string, string> Snapshot(JobTitle item) { return new Dictionary<string, string> { { "jobTitleCode", item.JobTitleCode }, { "jobTitleName", item.JobTitleName }, { "sortOrder", item.SortOrder.ToString() }, { "isActive", item.IsActive.ToString() } }; }
        private static AuditLog CreateAudit(int id, string code, string description, IDictionary<string, string> before, IDictionary<string, string> after)
        {
            return new AuditLog { UserId = CurrentAuthorization.UserId, ActionCode = code, EntityName = "dm_JobTitle", EntityId = id.ToString(), Description = description, MachineName = Environment.MachineName, DataJson = AuditDataJson.Change(before, after) };
        }
    }

    public sealed class JobTitleListItem
    {
        public int JobTitleId { get; set; }
        public string JobTitleCode { get; set; }
        public string JobTitleName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeCount { get; set; }
        public byte[] RowVersion { get; set; }
        public string StatusDisplay => IsActive ? "啟用" : "停用";
    }

    public sealed class JobTitleEditModel
    {
        public int? JobTitleId { get; set; }
        public string JobTitleCode { get; set; }
        public string JobTitleName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
