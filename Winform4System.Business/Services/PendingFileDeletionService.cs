using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public static class PendingFileDeletionService
    {
        private static int processing;
        private static Timer timer;

        public static void Start()
        {
            if (timer == null)
                timer = new Timer(_ => ProcessPending(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public static void ProcessInBackground()
        {
            Task.Run((Action)ProcessPending);
        }

        internal static string SpareFilePath(string folder, string storedName, int? ownerId = null)
        {
            if (string.IsNullOrWhiteSpace(storedName) || storedName == "." || storedName == ".."
                || storedName.IndexOfAny(new[] { '/', '\\' }) >= 0 || Path.IsPathRooted(storedName))
                throw new InvalidOperationException("儲存檔名不正確。");
            string root = Path.Combine(ShareDataStorage.CacheRoot, "SpareParts", folder);
            return ownerId.HasValue ? Path.Combine(root, ownerId.Value.ToString(), storedName) : Path.Combine(root, storedName);
        }

        internal static void Enqueue(SparePartDbContext context, string cachePath)
        {
            context.PendingFileDeletions.Add(new PendingFileDeletion
            {
                RelativePath = ShareDataStorage.RelativePath(cachePath),
                CreatedAt = DateTime.UtcNow,
                NextAttemptAt = DateTime.UtcNow
            });
        }

        public static void ProcessPending()
        {
            if (Interlocked.CompareExchange(ref processing, 1, 0) != 0) return;
            try
            {
                using (var context = new Winform4SystemDbContext(new ConnectionStringProvider().Get()))
                {
                    DateTime now = DateTime.UtcNow;
                    var pending = context.PendingFileDeletions.Where(x => x.NextAttemptAt <= now)
                        .OrderBy(x => x.Id).Take(20).ToList();
                    foreach (var item in pending)
                    {
                        try
                        {
                            ShareDataStorage.Delete(Path.Combine(ShareDataStorage.CacheRoot, item.RelativePath));
                            context.PendingFileDeletions.Remove(item);
                        }
                        catch (Exception ex)
                        {
                            item.AttemptCount++;
                            item.NextAttemptAt = DateTime.UtcNow.AddMinutes(5);
                            item.LastError = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { Trace.TraceError("Pending file deletion failed: {0}", ex.Message); }
            finally { Interlocked.Exchange(ref processing, 0); }
        }
    }
}
