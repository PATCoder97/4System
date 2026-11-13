using System;
using System.Collections.Generic;
using System.Linq;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.DataAccess.Repositories;

public static class VerifyModuleNavigation
{
    private static ApplicationFunction Item(string code, int? parent, string target, int sort = 0)
    {
        return new ApplicationFunction { FunctionCode = code, ParentFunctionId = parent, NavigationTarget = target,
            DisplayName = "導覽測試", SortOrder = sort, IsActive = true, IsVisible = true, DevelopmentStatus = "COMPLETED" };
    }

    public static void Main()
    {
        if (ShareDataStorage.EnvironmentName != "test") throw new Exception("Run only on Test");
        var provider = new ConnectionStringProvider();
        var service = new ModuleNavigationService(provider);
        var account = new EfUserAccountRepository(provider).FindByUserId("VNW0014732");
        CurrentAuthorization.SetIdentityAndPermissions(account.UserId, account.PermissionCodes);
        var registered = service.GetFunctions("ASSET.SPARE_PART");
        using (var db = new Winform4SystemDbContext(provider.Get()))
            if (db.ApplicationFunctions.Count(x => x.FunctionCode.StartsWith("ASSET.SPARE_PART.") && x.NavigationTarget != null) < 8)
                throw new Exception("Expected registered module functions");
        if (new EfMainMenuRepository(provider).GetAuthorizedMenuItems(account.UserId).Any(x => x.Code.StartsWith("ASSET.SPARE_PART.")))
            throw new Exception("Child function leaked into dashboard cards");

        string prefix = "VERIFY_NAV_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        var created = new List<int>();
        try
        {
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var root = Item(prefix, null, "SparePartForm"); db.ApplicationFunctions.Add(root); db.SaveChanges(); created.Add(root.FunctionId);
                var group = Item(prefix + ".GROUP", root.FunctionId, null);
                var hiddenGroup = Item(prefix + ".HIDDEN_GROUP", root.FunctionId, null); hiddenGroup.IsVisible = false;
                db.ApplicationFunctions.Add(group); db.ApplicationFunctions.Add(hiddenGroup); db.SaveChanges();
                created.Add(group.FunctionId); created.Add(hiddenGroup.FunctionId);
                var first = Item(prefix + ".FIRST", group.FunctionId, "SparePartMaterialView", 10);
                var second = Item(prefix + ".SECOND", group.FunctionId, "SparePartMachineView", 20);
                var denied = Item(prefix + ".DENIED", group.FunctionId, "SparePartMaterialView", 1);
                var hidden = Item(prefix + ".HIDDEN", root.FunctionId, "SparePartMaterialView"); hidden.IsVisible = false;
                var inactive = Item(prefix + ".INACTIVE", root.FunctionId, "SparePartMaterialView"); inactive.IsActive = false;
                var hiddenChild = Item(prefix + ".HIDDEN_CHILD", hiddenGroup.FunctionId, "SparePartMaterialView");
                var leaves = new[] { second, first, denied, hidden, inactive, hiddenChild };
                db.ApplicationFunctions.AddRange(leaves); db.SaveChanges(); created.AddRange(leaves.Select(x => x.FunctionId));
            }
            CurrentAuthorization.SetIdentityAndPermissions(account.UserId, new[] { "ASSET.SPARE_PART.VIEW", prefix + ".VIEW", prefix + ".FIRST.VIEW", prefix + ".SECOND.VIEW",
                prefix + ".HIDDEN.VIEW", prefix + ".INACTIVE.VIEW", prefix + ".HIDDEN_CHILD.VIEW" });
            var visible = service.GetFunctions(prefix);
            string[] targets = visible.Where(x => x.NavigationTarget != null && x.FunctionCode != prefix).Select(x => x.NavigationTarget).ToArray();
            if (!targets.SequenceEqual(new[] { "SparePartMaterialView", "SparePartMachineView" })) throw new Exception("Visibility, permission or ordering failed");
            bool rejected = false;
            try { service.DemandFunction(prefix, prefix + ".HIDDEN_CHILD"); } catch (UnauthorizedAccessException) { rejected = true; }
            if (!rejected) throw new Exception("Hidden ancestor was bypassed");
            CurrentAuthorization.SetIdentityAndPermissions(account.UserId, new[] { prefix + ".FIRST.VIEW" });
            rejected = false;
            try { service.GetFunctions(prefix); } catch (UnauthorizedAccessException) { rejected = true; }
            if (!rejected) throw new Exception("Module permission was bypassed");
            Console.WriteLine("PASS: 8 registered functions; dashboard isolation; ordering, visibility, inactive nodes, permission and ancestor guards");
        }
        finally
        {
            using (var db = new Winform4SystemDbContext(provider.Get()))
            {
                var remaining = db.ApplicationFunctions.Where(x => created.Contains(x.FunctionId)).ToList();
                while (remaining.Count > 0)
                {
                    var parents = new HashSet<int>(remaining.Where(x => x.ParentFunctionId.HasValue).Select(x => x.ParentFunctionId.Value));
                    var leaves = remaining.Where(x => !parents.Contains(x.FunctionId)).ToList();
                    db.ApplicationFunctions.RemoveRange(leaves); db.SaveChanges();
                    foreach (var leaf in leaves) remaining.Remove(leaf);
                }
            }
        }
    }
}
