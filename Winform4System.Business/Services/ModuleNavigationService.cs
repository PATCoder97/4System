using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Winform4System.Core.Security;
using Winform4System.Core.Models;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class ModuleNavigationService
    {
        private readonly string connectionString;

        public ModuleNavigationService(ConnectionStringProvider provider)
        {
            connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public IReadOnlyList<ApplicationFunction> GetFunctions(string moduleCode)
        {
            CurrentAuthorization.Demand(moduleCode + ".VIEW");
            using (var context = new Winform4SystemDbContext(connectionString))
            {
                var available = context.ApplicationFunctions.AsNoTracking()
                    .Where(x => x.IsActive && x.IsVisible).OrderBy(x => x.SortOrder).ThenBy(x => x.DisplayName).ToList();
                var root = available.SingleOrDefault(x => x.FunctionCode == moduleCode);
                if (root == null) return new List<ApplicationFunction>();
                var rootTarget = FunctionCatalog.Find(root.NavigationTarget);
                if (rootTarget == null || !rootTarget.IsModule) return new List<ApplicationFunction>();
                CurrentAuthorization.Demand(rootTarget.RequiredPermission);
                var ancestors = new HashSet<int> { root.FunctionId };
                var parentId = root.ParentFunctionId;
                while (parentId.HasValue)
                {
                    var parent = available.SingleOrDefault(x => x.FunctionId == parentId.Value);
                    if (parent == null || !ancestors.Add(parent.FunctionId)) return new List<ApplicationFunction>();
                    parentId = parent.ParentFunctionId;
                }
                var result = new List<ApplicationFunction> { root };
                var visited = new HashSet<int> { root.FunctionId };
                AddChildren(root, available, result, visited, rootTarget.Key);
                return result;
            }
        }

        private static void AddChildren(ApplicationFunction parent, IList<ApplicationFunction> available,
            ICollection<ApplicationFunction> result, ISet<int> visited, string host)
        {
            foreach (var child in available.Where(x => x.ParentFunctionId == parent.FunctionId))
            {
                if (!visited.Add(child.FunctionId)) continue;
                if (string.IsNullOrWhiteSpace(child.NavigationTarget))
                {
                    var children = new List<ApplicationFunction>();
                    AddChildren(child, available, children, visited, host);
                    if (children.Count == 0) continue;
                    result.Add(child);
                    foreach (var item in children) result.Add(item);
                }
                else if (CurrentAuthorization.HasPermission(child.FunctionCode + ".VIEW")
                    && FunctionCatalog.Find(child.NavigationTarget) != null
                    && FunctionCatalog.Find(child.NavigationTarget).Host == host
                    && CurrentAuthorization.HasPermission(FunctionCatalog.Find(child.NavigationTarget).RequiredPermission))
                {
                    result.Add(child);
                }
            }
        }

        public void DemandFunction(string moduleCode, string functionCode)
        {
            if (!GetFunctions(moduleCode).Any(x => x.FunctionCode == functionCode && !string.IsNullOrWhiteSpace(x.NavigationTarget)))
                throw new UnauthorizedAccessException("此功能已停用、隱藏或您沒有存取權限，請重新開啟模組。");
        }
    }
}
