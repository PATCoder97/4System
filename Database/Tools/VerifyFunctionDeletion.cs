using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.DataAccess.Repositories;

// Chỉ tạo/xóa fixture trong Test; không xóa chức năng thật hoặc đổi phiên của tài khoản thật.
public static class VerifyFunctionDeletion
{
    private sealed class FailFunctionDelete : DbCommandInterceptor
    {
        private static void Check(DbCommand command)
        {
            if (command.CommandText.IndexOf("DELETE", StringComparison.OrdinalIgnoreCase) >= 0
                && command.CommandText.Contains("\"app_Function\"")) throw new InvalidOperationException("Injected delete failure");
        }
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> context) { Check(command); }
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> context) { Check(command); }
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Rejected(Action action)
    {
        try { action(); } catch (InvalidOperationException) { return; } catch (UnauthorizedAccessException) { return; }
        throw new Exception("Expected rejection");
    }
    public static void Main()
    {
        if (ShareDataStorage.EnvironmentName != "test") throw new Exception("Test only");
        var provider = new ConnectionStringProvider();
        var service = new ApplicationFunctionService(provider);
        var admin = new EfUserAccountRepository(provider).FindByUserId("VNW0014732");
        Action login = () => CurrentAuthorization.SetIdentityAndPermissions(admin.UserId, admin.PermissionCodes);
        Func<int, ApplicationFunction> read = id => { using (var db = new Winform4SystemDbContext(provider.Get())) return db.ApplicationFunctions.AsNoTracking().Single(x => x.FunctionId == id); };
        string prefix = "VERIFY_DELETE_" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
        var created = new List<int>(); int roleId = 0, groupId = 0; string userId = null; bool userCreated = false;
        Guid stamp = Guid.NewGuid(), adminStamp;
        login();
        try
        {
            int parentId;
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                parentId = db.ApplicationFunctions.Single(x => x.FunctionCode == "ASSET.SPARE_PART").FunctionId;
                adminStamp = db.UserAccounts.Single(x => x.UserId == admin.UserId).SecurityStamp;
                foreach (string code in new[] { "MAIN", "SYSTEM.FUNCTION", "SYSTEM.ROLE", "ASSET.SPARE_PART" })
                    Rejected(() => service.Delete(db.ApplicationFunctions.AsNoTracking().Single(x => x.FunctionCode == code)));
                userId = "VNW" + new Random().Next(8000000, 9999999).ToString();
                if (db.UserAccounts.Any(x => x.UserId == userId)) throw new Exception("Fixture ID collision; rerun");
                db.UserAccounts.Add(new UserAccount { UserId = userId, SecurityStamp = stamp, IsActive = true, CreatedAt = DateTime.UtcNow });
                var role = new Role { RoleCode = prefix, RoleName = "刪除測試角色", IsActive = true };
                var group = new SecurityGroup { GroupCode = prefix, GroupName = "刪除測試群組", IsActive = true };
                db.Roles.Add(role); db.SecurityGroups.Add(group); db.SaveChanges(); userCreated = true; roleId = role.RoleId; groupId = group.GroupId;
                db.GroupRoles.Add(new GroupRole { GroupId = groupId, RoleId = roleId, AssignedAt = DateTime.UtcNow, IsActive = true });
                db.UserGroups.Add(new UserGroup { GroupId = groupId, UserId = userId, IsActive = true }); db.SaveChanges();
            }
            Func<string, int?, string, int> create = (code, parent, target) => {
                int id = service.Create(new ApplicationFunction { FunctionCode = code, ParentFunctionId = parent,
                    NavigationTarget = target, DisplayName = "刪除測試功能", IsActive = true, IsVisible = true, DevelopmentStatus = "COMPLETED" });
                created.Add(id); return id;
            };
            int branchId = create(prefix, parentId, null);
            int leafId = create(prefix + ".LEAF", branchId, "SparePartMaterialView");
            int keepId = create(prefix + ".KEEP", branchId, "SparePartMachineView");
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var ids = db.Permissions.Where(x => created.Contains(x.FunctionId)).Select(x => x.PermissionId).ToList();
                db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => ids.Contains(x.PermissionId))); db.SaveChanges();
                foreach (int id in ids) db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = id, AssignedAt = DateTime.UtcNow, IsActive = true });
                db.SaveChanges();
            }
            Rejected(() => service.Delete(read(branchId)));
            CurrentAuthorization.SetIdentityAndPermissions(admin.UserId, new[] { "SYSTEM.FUNCTION.VIEW" });
            Rejected(() => service.Delete(read(leafId))); login();
            var stale = read(leafId); var updated = read(leafId); updated.DisplayName = "已更新測試功能"; service.Update(updated);
            Rejected(() => service.Delete(stale));
            var interceptor = new FailFunctionDelete(); DbInterception.Add(interceptor);
            try { Rejected(() => service.Delete(read(leafId))); } finally { DbInterception.Remove(interceptor); }
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var permission = db.Permissions.Single(x => x.FunctionId == leafId);
                Assert(db.ApplicationFunctions.Any(x => x.FunctionId == leafId) && db.RolePermissions.Any(x => x.PermissionId == permission.PermissionId), "Rollback lost function/permission/grant");
                Assert(db.UserAccounts.Single(x => x.UserId == userId).SecurityStamp == stamp, "Rollback invalidated a session");
                Assert(!db.AuditLogs.Any(x => x.EntityId == leafId.ToString() && x.ActionCode == "SYSTEM.FUNCTION.DELETE"), "Rollback left successful audit");
            }
            service.Delete(read(leafId));
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                Assert(!db.ApplicationFunctions.Any(x => x.FunctionId == leafId) && !db.Permissions.Any(x => x.FunctionId == leafId), "Function or permission remained");
                Assert(db.RolePermissions.Count(x => x.RoleId == roleId) == 1 && db.Permissions.Any(x => x.FunctionId == keepId), "Unrelated grant was changed");
                Assert(db.UserAccounts.Single(x => x.UserId == userId).SecurityStamp != stamp, "Affected session still valid");
                Assert(db.UserAccounts.Single(x => x.UserId == admin.UserId).SecurityStamp == adminStamp, "Unrelated real account changed");
                var audit = db.AuditLogs.Single(x => x.EntityId == leafId.ToString() && x.ActionCode == "SYSTEM.FUNCTION.DELETE");
                Assert(audit.DataJson.Contains(prefix + ".LEAF.VIEW") && audit.DataJson.Contains(userId) && audit.DataJson.Contains("rolePermissionLinks"), "Missing deletion impact audit");
            }
            Rejected(() => service.Delete(stale));
            service.Delete(read(keepId)); service.Delete(read(branchId));
            Console.WriteLine("PASS: protected functions, children, authorization, stale/missing records; atomic rollback with injected failure; function/permission/grant deletion; retained unrelated data; affected-session invalidation and audit; empty-group deletion");
        }
        finally
        {
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var permissions = db.Permissions.Where(x => created.Contains(x.FunctionId)).ToList(); var ids = permissions.Select(x => x.PermissionId).ToList();
                db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => ids.Contains(x.PermissionId) || (roleId > 0 && x.RoleId == roleId))); db.SaveChanges();
                db.Permissions.RemoveRange(permissions); db.SaveChanges();
                foreach (int id in created.AsEnumerable().Reverse()) { var item = db.ApplicationFunctions.Find(id); if (item != null) { db.ApplicationFunctions.Remove(item); db.SaveChanges(); } }
                if (groupId > 0)
                {
                    db.UserGroups.RemoveRange(db.UserGroups.Where(x => x.GroupId == groupId));
                    db.GroupRoles.RemoveRange(db.GroupRoles.Where(x => x.GroupId == groupId)); db.SaveChanges();
                    db.SecurityGroups.RemoveRange(db.SecurityGroups.Where(x => x.GroupId == groupId));
                }
                if (roleId > 0) db.Roles.RemoveRange(db.Roles.Where(x => x.RoleId == roleId));
                if (userCreated) db.UserAccounts.RemoveRange(db.UserAccounts.Where(x => x.UserId == userId));
                var entityIds = created.Select(x => x.ToString()).ToList();
                db.AuditLogs.RemoveRange(db.AuditLogs.Where(x => x.EntityName == "app_Function" && entityIds.Contains(x.EntityId))); db.SaveChanges();
            }
            login();
        }
    }
}
