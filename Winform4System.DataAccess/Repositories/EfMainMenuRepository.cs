using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Models;

namespace Winform4System.DataAccess.Repositories
{
    public sealed class EfMainMenuRepository : IMainMenuRepository
    {
        private readonly string _connectionString;

        public EfMainMenuRepository(ConnectionStringProvider connectionStringProvider)
        {
            if (connectionStringProvider == null)
                throw new ArgumentNullException(nameof(connectionStringProvider));

            _connectionString = connectionStringProvider.Get();
        }

        public EfMainMenuRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public IReadOnlyList<MainMenuItemRecord> GetAuthorizedMenuItems(string userId)
        {
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                DateTime utcNow = DateTime.UtcNow;
                IQueryable<int> authorizedFunctionIds =
                    (from userGroup in context.UserGroups.AsNoTracking()
                     join securityGroup in context.SecurityGroups.AsNoTracking()
                         on userGroup.GroupId equals securityGroup.GroupId
                     join groupRole in context.GroupRoles.AsNoTracking()
                         on securityGroup.GroupId equals groupRole.GroupId
                     join role in context.Roles.AsNoTracking()
                         on groupRole.RoleId equals role.RoleId
                     join rolePermission in context.RolePermissions.AsNoTracking()
                         on role.RoleId equals rolePermission.RoleId
                     join permission in context.Permissions.AsNoTracking()
                         on rolePermission.PermissionId equals permission.PermissionId
                     where userGroup.UserId == userId
                           && userGroup.IsActive
                           && (!userGroup.ExpiresAt.HasValue || userGroup.ExpiresAt > utcNow)
                           && securityGroup.IsActive
                           && groupRole.IsActive
                           && role.IsActive
                           && rolePermission.IsActive
                           && permission.IsActive
                     select permission.FunctionId)
                    .Distinct();

                var allFunctions = context.ApplicationFunctions.AsNoTracking().ToList();
                return
                    (from functionData in context.ApplicationFunctions.AsNoTracking()
                     join parent in context.ApplicationFunctions.AsNoTracking()
                         on functionData.ParentFunctionId equals parent.FunctionId
                     where authorizedFunctionIds.Contains(functionData.FunctionId)
                           && functionData.IsActive
                           && functionData.IsVisible
                           && parent.IsActive
                           && parent.IsVisible
                     orderby parent.SortOrder, functionData.SortOrder, functionData.DisplayName
                     select new MainMenuItemRecord
                     {
                         Code = functionData.FunctionCode,
                         NavigationTarget = functionData.NavigationTarget,
                         IconKey = functionData.IconKey,
                         GroupName = parent.DisplayName,
                         Title = functionData.DisplayName,
                         Description = functionData.Description,
                         DevelopmentStatus = functionData.DevelopmentStatus,
                         IsWide = functionData.IsWide,
                         GroupSortOrder = parent.SortOrder,
                         SortOrder = functionData.SortOrder
                     })
                    .ToList().Where(x => (Winform4System.Core.Models.FunctionCatalog.Find(x.NavigationTarget)?.IsModule == true
                        || x.NavigationTarget == "MachineWarehouseForm") && HasVisibleAncestors(x.Code, allFunctions))
                    .ToList();
            }
        }
        private static bool HasVisibleAncestors(string code, System.Collections.Generic.IList<Winform4System.DataAccess.Entities.ApplicationFunction> all)
        {
            var current = all.SingleOrDefault(x => x.FunctionCode == code);
            var seen = new System.Collections.Generic.HashSet<int>();
            while (current != null)
            {
                if (!current.IsActive || !current.IsVisible || !seen.Add(current.FunctionId)) return false;
                if (!current.ParentFunctionId.HasValue) return true;
                current = all.SingleOrDefault(x => x.FunctionId == current.ParentFunctionId.Value);
            }
            return false;
        }
    }
}
