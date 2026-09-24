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
    public sealed class AuditLogService
    {
        public const int MaximumRows = 5000;
        private readonly string _connectionString;

        public AuditLogService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public AuditLogSearchResult Search(AuditLogFilter filter)
        {
            CurrentAuthorization.Demand("SYSTEM.AUDIT.VIEW");
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (filter.ToUtc <= filter.FromUtc) throw new InvalidOperationException("結束日期必須晚於開始日期。");
            if ((filter.ToUtc - filter.FromUtc).TotalDays > 366) throw new InvalidOperationException("單次查詢期間不可超過一年。");

            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                var query = context.AuditLogs.AsNoTracking()
                    .Where(x => x.CreatedAt >= filter.FromUtc && x.CreatedAt < filter.ToUtc);

                if (!string.IsNullOrWhiteSpace(filter.UserKeyword))
                {
                    string keyword = filter.UserKeyword.Trim();
                    query = query.Where(x => x.UserId.Contains(keyword) || x.AttemptedUserId.Contains(keyword));
                }
                if (!string.IsNullOrWhiteSpace(filter.ActionCode))
                {
                    string action = filter.ActionCode.Trim();
                    query = query.Where(x => x.ActionCode == action);
                }
                if (!string.IsNullOrWhiteSpace(filter.EntityName))
                {
                    string entity = filter.EntityName.Trim();
                    query = query.Where(x => x.EntityName == entity);
                }
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    string keyword = filter.Keyword.Trim();
                    query = query.Where(x => x.Description.Contains(keyword) || x.EntityId.Contains(keyword));
                }

                var rows = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.AuditLogId)
                    .Select(x => new AuditLogListItem
                    {
                        AuditLogId = x.AuditLogId,
                        CreatedAtUtc = x.CreatedAt,
                        UserId = x.UserId,
                        AttemptedUserId = x.AttemptedUserId,
                        ActionCode = x.ActionCode,
                        EntityName = x.EntityName,
                        EntityId = x.EntityId,
                        Description = x.Description,
                        MachineName = x.MachineName,
                        IpAddress = x.IpAddress,
                        CorrelationId = x.CorrelationId,
                        DataJson = x.DataJson
                    }).Take(MaximumRows + 1).ToList();

                var userIds = rows.Where(x => !string.IsNullOrWhiteSpace(x.UserId)).Select(x => x.UserId).Distinct().ToList();
                var names = (from account in context.UserAccounts.AsNoTracking()
                             join employee in context.EmployeeProfiles.AsNoTracking() on account.EmployeeProfileId equals employee.EmployeeProfileId
                             where userIds.Contains(account.UserId)
                             select new { account.UserId, employee.DisplayNameTW, employee.DisplayNameVN }).ToList()
                    .ToDictionary(x => x.UserId, x => !string.IsNullOrWhiteSpace(x.DisplayNameTW) ? x.DisplayNameTW : x.DisplayNameVN, StringComparer.OrdinalIgnoreCase);

                bool truncated = rows.Count > MaximumRows;
                if (truncated) rows.RemoveAt(rows.Count - 1);
                foreach (var row in rows)
                {
                    string name;
                    row.UserDisplayName = !string.IsNullOrWhiteSpace(row.UserId) && names.TryGetValue(row.UserId, out name) ? name : row.UserId;
                }
                return new AuditLogSearchResult { Items = rows, IsTruncated = truncated };
            }
        }

        public AuditLogFilterOptions GetFilterOptions()
        {
            CurrentAuthorization.Demand("SYSTEM.AUDIT.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return new AuditLogFilterOptions
                {
                    ActionCodes = context.AuditLogs.AsNoTracking().Select(x => x.ActionCode).Distinct().OrderBy(x => x).ToList(),
                    EntityNames = context.AuditLogs.AsNoTracking().Where(x => x.EntityName != null).Select(x => x.EntityName).Distinct().OrderBy(x => x).ToList()
                };
            }
        }

        public void RecordExport(int rowCount)
        {
            CurrentAuthorization.Demand("SYSTEM.AUDIT.EXPORT");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = "AUDIT.LOG.EXPORT",
                    EntityName = "audit_AuditLog",
                    Description = "匯出稽核記錄，共 " + rowCount + " 筆",
                    MachineName = Environment.MachineName
                });
                context.SaveChanges();
            }
        }
    }

    public sealed class AuditLogFilter
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
        public string UserKeyword { get; set; }
        public string ActionCode { get; set; }
        public string EntityName { get; set; }
        public string Keyword { get; set; }
    }

    public sealed class AuditLogSearchResult
    {
        public IReadOnlyList<AuditLogListItem> Items { get; set; }
        public bool IsTruncated { get; set; }
    }

    public sealed class AuditLogFilterOptions
    {
        public IReadOnlyList<string> ActionCodes { get; set; }
        public IReadOnlyList<string> EntityNames { get; set; }
    }

    public sealed class AuditLogListItem
    {
        public long AuditLogId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime CreatedAtLocal => DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc).ToLocalTime();
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string AttemptedUserId { get; set; }
        public string ActionCode { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string Description { get; set; }
        public string MachineName { get; set; }
        public string IpAddress { get; set; }
        public Guid CorrelationId { get; set; }
        public string DataJson { get; set; }
    }
}
