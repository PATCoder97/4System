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
    public sealed class ApplicationFunctionService
    {
        private static readonly HashSet<string> ValidDevelopmentStatuses =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "NOT_STARTED",
                "IN_PROGRESS",
                "COMPLETED"
            };

        private readonly string _connectionString;

        public ApplicationFunctionService(ConnectionStringProvider connectionStringProvider)
        {
            if (connectionStringProvider == null)
                throw new ArgumentNullException(nameof(connectionStringProvider));

            _connectionString = connectionStringProvider.Get();
        }

        public IReadOnlyList<ApplicationFunction> GetList()
        {
            CurrentAuthorization.Demand("SYSTEM.FUNCTION.VIEW");

            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.ApplicationFunctions
                    .AsNoTracking()
                    .OrderBy(item => item.ParentFunctionId.HasValue ? 1 : 0)
                    .ThenBy(item => item.SortOrder)
                    .ThenBy(item => item.DisplayName)
                    .ToList();
            }
        }

        public void Update(ApplicationFunction item)
        {
            CurrentAuthorization.Demand("SYSTEM.FUNCTION.ADMIN");
            Validate(item);

            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                var existing = context.ApplicationFunctions
                    .FirstOrDefault(candidate => candidate.FunctionId == item.FunctionId);
                if (existing == null)
                    throw new InvalidOperationException("找不到需要更新的功能資料。");

                if (item.ParentFunctionId.HasValue)
                {
                    bool parentExists = context.ApplicationFunctions
                        .Any(candidate => candidate.FunctionId == item.ParentFunctionId.Value);
                    if (!parentExists)
                        throw new InvalidOperationException("找不到所選的上層功能。");

                    EnsureParentDoesNotCreateCycle(context, item.FunctionId, item.ParentFunctionId.Value);
                }

                existing.ParentFunctionId = item.ParentFunctionId;
                existing.DisplayName = item.DisplayName.Trim();
                existing.Description = NormalizeOptionalText(item.Description);
                existing.NavigationTarget = NormalizeOptionalText(item.NavigationTarget);
                existing.SortOrder = item.SortOrder;
                existing.IsVisible = item.IsVisible;
                existing.IsActive = item.IsActive;
                existing.DevelopmentStatus = item.DevelopmentStatus.Trim().ToUpperInvariant();
                existing.IsWide = item.IsWide;
                existing.UpdatedAt = DateTime.UtcNow;

                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void Validate(ApplicationFunction item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.FunctionId <= 0)
                throw new InvalidOperationException("功能識別碼無效。");
            if (string.IsNullOrWhiteSpace(item.DisplayName))
                throw new InvalidOperationException("請輸入功能名稱。");
            if (item.DisplayName.Trim().Length > 200)
                throw new InvalidOperationException("功能名稱不可超過 200 個字元。");
            if (!string.IsNullOrWhiteSpace(item.Description) && item.Description.Trim().Length > 500)
                throw new InvalidOperationException("功能說明不可超過 500 個字元。");
            if (!string.IsNullOrWhiteSpace(item.NavigationTarget) && item.NavigationTarget.Trim().Length > 300)
                throw new InvalidOperationException("導覽目標不可超過 300 個字元。");
            if (item.SortOrder < 0)
                throw new InvalidOperationException("排序值不可小於零。");
            if (!ValidDevelopmentStatuses.Contains(item.DevelopmentStatus ?? string.Empty))
                throw new InvalidOperationException("開發狀態無效。");
            if (item.ParentFunctionId == item.FunctionId)
                throw new InvalidOperationException("功能不可將自己設為上層功能。");
        }

        private static void EnsureParentDoesNotCreateCycle(
            Winform4SystemDbContext context,
            int functionId,
            int parentFunctionId)
        {
            int? currentId = parentFunctionId;
            var visited = new HashSet<int>();

            while (currentId.HasValue)
            {
                if (currentId.Value == functionId)
                    throw new InvalidOperationException("所選的上層功能會造成循環關聯。");
                if (!visited.Add(currentId.Value))
                    throw new InvalidOperationException("功能階層資料已有循環關聯。");

                currentId = context.ApplicationFunctions
                    .Where(candidate => candidate.FunctionId == currentId.Value)
                    .Select(candidate => candidate.ParentFunctionId)
                    .FirstOrDefault();
            }
        }

        private static string NormalizeOptionalText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
