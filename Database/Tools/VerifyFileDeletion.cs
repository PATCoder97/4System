using System;
using System.IO;
using System.Linq;
using System.Threading;
using Winform4System.Business.Services;
using Winform4System.Business.Services.SpareParts;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.DataAccess.Entities.SpareParts;

public static class VerifyFileDeletion
{
    public static void Main()
    {
        if (ShareDataStorage.EnvironmentName != "test") throw new Exception("Run only on Test");
        CurrentAuthorization.SetIdentityAndPermissions("VNW0014732", new[] { "ASSET.SPARE_PART.ADMIN" });
        string connection = new ConnectionStringProvider().Get();
        string filename = Guid.NewGuid().ToString("N") + ".bin";
        string relative = "SpareParts/RecoveryGuides/" + filename;
        string target = Path.Combine(ShareDataStorage.CacheRoot, "SpareParts", "RecoveryGuides", filename);
        string source = Path.GetTempFileName();
        int guideId = 0;
        try
        {
            File.WriteAllBytes(source, new byte[] { 0, 127, 255 });
            ShareDataStorage.Upload(source, target);
            using (var db = new SparePartDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                db.PendingFileDeletions.Add(new PendingFileDeletion { RelativePath = relative, CreatedAt = DateTime.UtcNow, NextAttemptAt = DateTime.UtcNow });
                db.SaveChanges();
                transaction.Rollback();
            }
            using (var db = new Winform4SystemDbContext(connection))
                if (db.PendingFileDeletions.Any(x => x.RelativePath == relative)) throw new Exception("Queue survived rollback");
            ShareDataStorage.Download(target);
            using (var db = new SparePartDbContext())
            {
                var guide = new SparePartRecoveryGuide { Title = "刪除測試", EncryptionName = filename, ActualName = filename, UploadedDate = DateTime.Now, UploadedBy = "VNW0014732", IsActive = true };
                db.RecoveryGuides.Add(guide);
                db.SaveChanges();
                guideId = guide.Id;
            }
            if (!SparePartRecoveryService.Instance.DeactivateGuide(guideId)) throw new Exception("Guide deletion failed");
            bool pending = true;
            for (int attempt = 0; attempt < 50 && pending; attempt++)
            {
                PendingFileDeletionService.ProcessPending();
                using (var db = new Winform4SystemDbContext(connection))
                    pending = db.PendingFileDeletions.Any(x => x.RelativePath == relative);
                if (pending) Thread.Sleep(100);
            }
            if (pending) throw new Exception("Queue did not drain");
            using (var db = new SparePartDbContext())
                if (db.RecoveryGuides.Single(x => x.Id == guideId).IsActive) throw new Exception("Guide still active");
            bool missing = false;
            try { ShareDataStorage.Download(target); } catch (FileNotFoundException) { missing = true; }
            if (!missing) throw new Exception("File survived guide deletion");
            bool blocked = false;
            try { ShareDataStorage.Delete(Path.Combine(ShareDataStorage.CacheRoot, "..", "outside.bin")); }
            catch (InvalidOperationException) { blocked = true; }
            if (!blocked) throw new Exception("Path traversal was accepted");
            string invalidPath = "../" + filename;
            using (var db = new Winform4SystemDbContext(connection))
            {
                db.PendingFileDeletions.Add(new PendingFileDeletion { RelativePath = invalidPath, CreatedAt = DateTime.UtcNow, NextAttemptAt = DateTime.UtcNow });
                db.SaveChanges();
            }
            PendingFileDeletionService.ProcessPending();
            using (var db = new Winform4SystemDbContext(connection))
            {
                var retry = db.PendingFileDeletions.Single(x => x.RelativePath == invalidPath);
                if (retry.AttemptCount != 1 || retry.NextAttemptAt <= DateTime.UtcNow || string.IsNullOrWhiteSpace(retry.LastError))
                    throw new Exception("Failed deletion was not scheduled for retry");
                db.PendingFileDeletions.Remove(retry);
                db.SaveChanges();
            }
            Console.WriteLine("PASS: rollback, guide deletion, queue drain, path guard and retry scheduling");
        }
        finally
        {
            File.Delete(source);
            ShareDataStorage.Delete(target);
            using (var db = new SparePartDbContext())
            {
                var guide = db.RecoveryGuides.SingleOrDefault(x => x.Id == guideId);
                if (guide != null) { db.RecoveryGuides.Remove(guide); db.SaveChanges(); }
            }
        }
    }
}
