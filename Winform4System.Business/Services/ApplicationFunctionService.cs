using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Winform4System.Core.Models;
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

        public int Create(ApplicationFunction item)
        {
            CurrentAuthorization.Demand("SYSTEM.FUNCTION.ADMIN");
            Validate(item, true);
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction())
            {
                string code = (item.FunctionCode ?? string.Empty).Trim().ToUpperInvariant();
                if (!Regex.IsMatch(code, @"^[A-Z][A-Z0-9_]*(\.[A-Z][A-Z0-9_]*)*$") || code.Length > 100)
                    throw new InvalidOperationException("功能代碼必須以英文字母開頭，僅能使用大寫字母、數字、底線及分隔點，長度不可超過 100。");
                if (context.ApplicationFunctions.Any(x => x.FunctionCode == code))
                    throw new InvalidOperationException("此功能代碼已存在。");
                var added = new ApplicationFunction
                {
                    FunctionCode = code, ParentFunctionId = item.ParentFunctionId, DisplayName = item.DisplayName.Trim(),
                    Description = NormalizeOptionalText(item.Description), NavigationTarget = NormalizeOptionalText(item.NavigationTarget),
                    IconKey = NormalizeOptionalText(item.IconKey), SortOrder = item.SortOrder, IsWide = item.IsWide,
                    IsActive = item.IsActive, IsVisible = item.IsVisible, DevelopmentStatus = item.DevelopmentStatus, UpdatedAt = DateTime.UtcNow
                };
                ValidatePlacement(added, context.ApplicationFunctions.ToList(), false);
                context.ApplicationFunctions.Add(added);
                context.SaveChanges();
                EnsureViewPermission(context, added);
                context.AuditLogs.Add(new AuditLog { UserId = CurrentAuthorization.UserId, ActionCode = "SYSTEM.FUNCTION.CREATE",
                    EntityName = "app_Function", EntityId = added.FunctionId.ToString(), Description = "新增功能設定 " + code,
                    MachineName = Environment.MachineName, DataJson = AuditDataJson.Change(new Dictionary<string, string>(), CreateAuditSnapshot(added)) });
                context.SaveChanges();
                transaction.Commit();
                return added.FunctionId;
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
                if (existing.RowVersion == null || item.RowVersion == null || !existing.RowVersion.SequenceEqual(item.RowVersion))
                    throw new InvalidOperationException("此功能資料已由其他使用者更新，請重新載入後再試。");

                if (!string.Equals(existing.FunctionCode, item.FunctionCode, StringComparison.Ordinal))
                    throw new InvalidOperationException("建立後不可變更功能代碼。");
                string previousTarget = existing.NavigationTarget;
                var before = CreateAuditSnapshot(existing);

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
                existing.IconKey = NormalizeOptionalText(item.IconKey);
                existing.SortOrder = item.SortOrder;
                existing.IsVisible = item.IsVisible;
                existing.IsActive = item.IsActive;
                existing.DevelopmentStatus = item.DevelopmentStatus.Trim().ToUpperInvariant();
                existing.IsWide = item.IsWide;
                existing.UpdatedAt = DateTime.UtcNow;

                var all = context.ApplicationFunctions.ToList();
                ValidatePlacement(existing, all, previousTarget == existing.NavigationTarget);
                foreach (var descendant in all.Where(x => IsDescendant(x, existing.FunctionId, all)))
                    ValidatePlacement(descendant, all, true);
                EnsureViewPermission(context, existing);
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = "SYSTEM.FUNCTION.UPDATE",
                    EntityName = "app_Function",
                    EntityId = existing.FunctionId.ToString(),
                    Description = "更新功能設定 " + existing.FunctionCode,
                    MachineName = Environment.MachineName,
                    DataJson = AuditDataJson.Change(before, CreateAuditSnapshot(existing))
                });

                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void Validate(ApplicationFunction item, bool isNew = false)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!isNew && item.FunctionId <= 0)
                throw new InvalidOperationException("功能識別碼無效。");
            if (string.IsNullOrWhiteSpace(item.DisplayName))
                throw new InvalidOperationException("請輸入功能名稱。");
            if (item.DisplayName.Trim().Length > 200)
                throw new InvalidOperationException("功能名稱不可超過 200 個字元。");
            if (!string.IsNullOrWhiteSpace(item.Description) && item.Description.Trim().Length > 500)
                throw new InvalidOperationException("功能說明不可超過 500 個字元。");
            if (!string.IsNullOrWhiteSpace(item.NavigationTarget) && item.NavigationTarget.Trim().Length > 300)
                throw new InvalidOperationException("導覽目標不可超過 300 個字元。");
            if (!string.IsNullOrWhiteSpace(item.IconKey) && !FunctionCatalog.Icons.ContainsKey(item.IconKey))
                throw new InvalidOperationException("請從共用圖示清單選擇圖示。");
            if (item.SortOrder < 0)
                throw new InvalidOperationException("排序值不可小於零。");
            if (!ValidDevelopmentStatuses.Contains(item.DevelopmentStatus ?? string.Empty))
                throw new InvalidOperationException("開發狀態無效。");
            if (!isNew && item.ParentFunctionId == item.FunctionId)
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

        private static void EnsureViewPermission(Winform4SystemDbContext context, ApplicationFunction function)
        {
            if (string.IsNullOrWhiteSpace(function.NavigationTarget)) return;
            string code = function.FunctionCode + ".VIEW";
            if (context.Permissions.Any(x => x.PermissionCode == code)) return;
            string label = "檢視" + function.DisplayName;
            var permission = new Permission { FunctionId = function.FunctionId, ActionCode = "VIEW", PermissionCode = code,
                DisplayName = label.Length > 200 ? label.Substring(0, 200) : label, IsActive = true };
            context.Permissions.Add(permission);
            context.SaveChanges();
            var adminRole = context.Roles.SingleOrDefault(x => x.RoleCode == "SECURITY_ADMIN");
            if (adminRole != null)
                context.RolePermissions.Add(new RolePermission { RoleId = adminRole.RoleId, PermissionId = permission.PermissionId,
                    IsActive = true, AssignedAt = DateTime.UtcNow, AssignedByUserId = CurrentAuthorization.UserId });
        }

        private static bool IsDescendant(ApplicationFunction item, int ancestorId, IList<ApplicationFunction> all)
        {
            var seen = new HashSet<int>();
            int? parentId = item.ParentFunctionId;
            while (parentId.HasValue && seen.Add(parentId.Value))
            {
                if (parentId.Value == ancestorId) return true;
                var parent = all.FirstOrDefault(x => x.FunctionId == parentId.Value);
                parentId = parent?.ParentFunctionId;
            }
            return false;
        }

        private static void ValidatePlacement(ApplicationFunction item, IList<ApplicationFunction> all, bool allowLegacy)
        {
            var definition = FunctionCatalog.Find(item.NavigationTarget);
            if (!string.IsNullOrWhiteSpace(item.NavigationTarget) && definition == null && !allowLegacy)
                throw new InvalidOperationException("請選擇已實作的導覽目標，或留空作為群組。");
            FunctionTarget host = null;
            int? parentId = item.ParentFunctionId;
            var seen = new HashSet<int>();
            while (parentId.HasValue)
            {
                if (!seen.Add(parentId.Value) || (item.FunctionId > 0 && parentId.Value == item.FunctionId))
                    throw new InvalidOperationException("功能階層不可包含循環關聯。");
                var parent = all.FirstOrDefault(x => x.FunctionId == parentId.Value);
                if (parent == null) throw new InvalidOperationException("找不到所選的上層功能。");
                var parentTarget = FunctionCatalog.Find(parent.NavigationTarget);
                if (parentTarget != null)
                {
                    if (!parentTarget.IsModule) throw new InvalidOperationException("畫面功能不能包含子功能，請選擇群組或模組。");
                    if (host == null) host = parentTarget;
                }
                parentId = parent.ParentFunctionId;
            }
            if (definition == null) return;
            if (definition.IsModule && (!item.ParentFunctionId.HasValue || host != null))
                throw new InvalidOperationException("模組必須放在主畫面群組下，不能放在其他模組內。");
            if (!definition.IsModule && (host == null || host.Key != definition.Host))
                throw new InvalidOperationException("導覽目標與所選模組不相容，請選擇正確的上層群組。");
        }

        private static string NormalizeOptionalText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static Dictionary<string, string> CreateAuditSnapshot(ApplicationFunction item)
        {
            return new Dictionary<string, string>
            {
                { "parentFunctionId", item.ParentFunctionId?.ToString() },
                { "displayName", item.DisplayName },
                { "navigationTarget", item.NavigationTarget },
                { "iconKey", item.IconKey },
                { "sortOrder", item.SortOrder.ToString() },
                { "developmentStatus", item.DevelopmentStatus },
                { "isWide", item.IsWide.ToString() },
                { "isVisible", item.IsVisible.ToString() },
                { "isActive", item.IsActive.ToString() }
            };
        }
    }
}
