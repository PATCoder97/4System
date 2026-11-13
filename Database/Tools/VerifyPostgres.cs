using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.Business.Services;

public static class VerifyPostgres
{
    public static void Main()
    {
        if (ShareDataStorage.EnvironmentName != "test") throw new InvalidOperationException("Verification must run on Test.");
        string connection = new ConnectionStringProvider().Get();
        int id;
        using (var db = new Winform4SystemDbContext(connection))
        {
            Console.WriteLine("Functions: " + db.ApplicationFunctions.Count() + "; permissions: " + db.Permissions.Count());
            var item = new Department { DepartmentCode = "VERIFY_" + Guid.NewGuid().ToString("N").Substring(0, 12), DepartmentName = "連線測試", IsActive = true };
            db.Departments.Add(item);
            db.SaveChanges();
            id = item.DepartmentId;
            if (item.RowVersion == null || item.RowVersion.Length != 8) throw new Exception("RowVersion missing");
        }
        using (var prod = new Winform4SystemDbContext(new ConnectionStringProvider().Get("ProdDatabase")))
            Console.WriteLine("Prod read-only: " + prod.ApplicationFunctions.Count() + " functions");
        try
        {
            using (var first = new Winform4SystemDbContext(connection))
            using (var second = new Winform4SystemDbContext(connection))
            {
                var a = first.Departments.Single(x => x.DepartmentId == id);
                var b = second.Departments.Single(x => x.DepartmentId == id);
                a.DepartmentName = "已更新";
                first.SaveChanges();
                b.DepartmentName = "衝突測試";
                bool rejected = false;
                try { second.SaveChanges(); } catch (DbUpdateConcurrencyException) { rejected = true; }
                if (!rejected) throw new Exception("Stale update was accepted");
            }
            using (var db = new Winform4SystemDbContext(connection))
            using (var transaction = db.Database.BeginTransaction())
            {
                var audit = new AuditLog { ActionCode = "VERIFY.POSTGRES", Description = "資料庫連線測試" };
                db.AuditLogs.Add(audit);
                db.SaveChanges();
                if (audit.CreatedAt == default(DateTime) || audit.CorrelationId == Guid.Empty) throw new Exception("Audit defaults missing");
                transaction.Rollback();
            }
            using (var spare = new SparePartDbContext())
            {
                Console.WriteLine("Spare materials: " + spare.Materials.Count() + "; users view: " + spare.Users.Count() + "; departments view: " + spare.Departments.Count());
            }
            ShareDataStorage.CheckHealthAsync().GetAwaiter().GetResult();
            string source = Path.GetTempFileName();
            string target = Path.Combine(ShareDataStorage.CacheRoot, "Diagnostics", Guid.NewGuid().ToString("N") + ".bin");
            try
            {
                byte[] bytes = Enumerable.Range(0, 1024).Select(x => (byte)(x % 256)).ToArray();
                File.WriteAllBytes(source, bytes);
                ShareDataStorage.Upload(source, target);
                ShareDataStorage.Download(target);
                if (!bytes.SequenceEqual(File.ReadAllBytes(target))) throw new Exception("Binary roundtrip mismatch");
                ShareDataStorage.Delete(target);
                ShareDataStorage.Delete(target); // DELETE 404 có thể thử lại an toàn.
                bool missing = false;
                try { ShareDataStorage.Download(target); } catch (FileNotFoundException) { missing = true; }
                if (!missing || File.Exists(target)) throw new Exception("Deleted file still exists");
                Console.WriteLine("API upload/download/delete and repeated delete: PASS");
            }
            finally { File.Delete(source); ShareDataStorage.Delete(target); }
            Console.WriteLine("EF6 insert, update, conflict rejection, audit defaults and API: PASS");
        }
        finally
        {
            using (var db = new Winform4SystemDbContext(connection))
            {
                db.Departments.Remove(db.Departments.Single(x => x.DepartmentId == id));
                db.SaveChanges();
            }
        }
    }
}
