using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.DataAccess.Repositories;

public static class VerifyManualFunctions
{
    private static ApplicationFunction Item(string code, int? parent, string target = null)
    {
        return new ApplicationFunction { FunctionCode = code, ParentFunctionId = parent, NavigationTarget = target,
            DisplayName = "手動功能測試", IconKey = "Search", IsActive = true, IsVisible = true, DevelopmentStatus = "COMPLETED" };
    }
    private static void Rejected(Action action)
    {
        try { action(); } catch (InvalidOperationException) { return; } catch (UnauthorizedAccessException) { return; }
        throw new Exception("Expected operation to be rejected");
    }
    public static void Main()
    {
        if (ShareDataStorage.EnvironmentName != "test") throw new Exception("Run only on Test");
        var provider = new ConnectionStringProvider();
        var service = new ApplicationFunctionService(provider);
        var nav = new ModuleNavigationService(provider);
        string prefix = "VERIFY_MANUAL_" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
        var created = new List<int>();
        Action login = () => { var account = new EfUserAccountRepository(provider).FindByUserId("VNW0014732"); CurrentAuthorization.SetIdentityAndPermissions(account.UserId, account.PermissionCodes); };
        Func<int, ApplicationFunction> read = id => { using (var db = new Winform4SystemDbContext(provider.Get())) return db.ApplicationFunctions.AsNoTracking().Single(x => x.FunctionId == id); };
        login();
        try
        {
            int rootId, systemId, categoryId;
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                rootId = db.ApplicationFunctions.Single(x => x.FunctionCode == "ASSET.SPARE_PART").FunctionId;
                systemId = db.ApplicationFunctions.Single(x => x.FunctionCode == "SYSTEM.SETTINGS").FunctionId;
                categoryId = db.ApplicationFunctions.Single(x => x.FunctionCode == "ASSET").FunctionId;
            }
            int groupId = service.Create(Item(prefix, rootId)); created.Add(groupId);
            int leafId = service.Create(Item(prefix + ".LEAF", groupId, "SparePartMaterialView")); created.Add(leafId);
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var permission = db.Permissions.Single(x => x.PermissionCode == prefix + ".LEAF.VIEW");
                if (!db.RolePermissions.Any(x => x.PermissionId == permission.PermissionId && x.IsActive)) throw new Exception("Missing admin assignment");
                if (read(leafId).IconKey != "Search") throw new Exception("Icon did not persist");
                if (!db.AuditLogs.Any(x => x.EntityId == leafId.ToString() && x.ActionCode == "SYSTEM.FUNCTION.CREATE")) throw new Exception("Missing create audit");
            }
            Rejected(() => service.Create(Item(prefix + ".LEAF", groupId, "SparePartMaterialView")));
            Rejected(() => service.Create(Item(prefix + ".UNKNOWN", groupId, "UnregisteredScreen")));
            var invalidIcon = Item(prefix + ".ICON", groupId); invalidIcon.IconKey = "../../icon"; Rejected(() => service.Create(invalidIcon));
            var cycle = read(groupId); cycle.ParentFunctionId = leafId; Rejected(() => service.Update(cycle));
            var wrongHost = read(groupId); wrongHost.ParentFunctionId = systemId; Rejected(() => service.Update(wrongHost));
            if (read(groupId).ParentFunctionId != rootId) throw new Exception("Rejected move changed data");
            var stale = read(leafId); var edit = read(leafId); edit.DisplayName = "更新名稱"; edit.IconKey = "Equipment"; service.Update(edit);
            Rejected(() => service.Update(stale));
            int moduleId = service.Create(Item(prefix + ".MODULE", categoryId, "SparePartForm")); created.Add(moduleId);
            int aliasId = service.Create(Item(prefix + ".ALIAS", moduleId, "SparePartMaterialView")); created.Add(aliasId);
            login();
            if (!nav.GetFunctions(prefix + ".MODULE").Any(x => x.FunctionId == aliasId)) throw new Exception("Manual module target did not resolve");
            if (!new EfMainMenuRepository(provider).GetAuthorizedMenuItems("VNW0014732").Any(x => x.Code == prefix + ".MODULE")) throw new Exception("Manual card missing");
            CurrentAuthorization.SetIdentityAndPermissions("VNW0014732", new[] { "ASSET.SPARE_PART.VIEW", prefix + ".LEAF.VIEW" });
            if (!nav.GetFunctions("ASSET.SPARE_PART").Any(x => x.FunctionId == leafId)) throw new Exception("Manual leaf missing");
            CurrentAuthorization.SetIdentityAndPermissions("VNW0014732", new[] { prefix + ".MODULE.VIEW", prefix + ".ALIAS.VIEW" });
            Rejected(() => nav.GetFunctions(prefix + ".MODULE"));
            Rejected(() => service.Create(Item(prefix + ".DENIED", rootId)));
            Console.WriteLine("PASS: manual create/edit, icon, VIEW permission, audit, duplicate/target/icon validation, cycle/host guard, concurrency, module alias and authorization");
        }
        finally
        {
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var permissions = db.Permissions.Where(x => created.Contains(x.FunctionId)).ToList();
                var ids = permissions.Select(x => x.PermissionId).ToList();
                db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => ids.Contains(x.PermissionId))); db.SaveChanges();
                db.Permissions.RemoveRange(permissions); db.SaveChanges();
                var remaining = db.ApplicationFunctions.Where(x => created.Contains(x.FunctionId)).ToList();
                while (remaining.Count > 0)
                {
                    var parents = new HashSet<int>(remaining.Where(x => x.ParentFunctionId.HasValue).Select(x => x.ParentFunctionId.Value));
                    var leaves = remaining.Where(x => !parents.Contains(x.FunctionId)).ToList();
                    db.ApplicationFunctions.RemoveRange(leaves); db.SaveChanges();
                    foreach (var leaf in leaves) remaining.Remove(leaf);
                }
                var entityIds = created.Select(x => x.ToString()).ToList();
                db.AuditLogs.RemoveRange(db.AuditLogs.Where(x => x.EntityName == "app_Function" && entityIds.Contains(x.EntityId)));
                db.SaveChanges();
            }
        }
    }
}
