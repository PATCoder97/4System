using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Entities.SpareParts;
using Winform4System.Business.Services.SpareParts;

namespace Winform4System.Business.Services.SpareParts
{
    public class SparePartInspectionBatchService
    {
        SparePartLogger logger;

        private static SparePartInspectionBatchService instance;

        public static SparePartInspectionBatchService Instance
        {
            get { if (instance == null) instance = new SparePartInspectionBatchService(); return instance; }
            private set { instance = value; }
        }

        private SparePartInspectionBatchService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartInspectionBatch> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionBatches.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartInspectionBatch GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionBatches.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(SparePartInspectionBatch item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionBatches.Add(item);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return item.Id;
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<SparePartInspectionBatch> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionBatches.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(SparePartInspectionBatch item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionBatches.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool CancelBatch(int batchId, string currentUserId, string cancelReason, out string message)
        {
            message = string.Empty;

            try
            {
                Winform4System.Core.Security.CurrentAuthorization.Demand("ASSET.SPARE_PART.APPROVE");
                using (var _context = new SparePartDbContext())
                {
                    var batch = _context.InspectionBatches.FirstOrDefault(item => item.Id == batchId);
                    if (batch == null)
                    {
                        message = "找不到對應盤點批次。";
                        return false;
                    }

                    if (batch.IsCancelled)
                    {
                        message = "此批次已取消。";
                        return false;
                    }

                    bool isCompleted = !_context.InspectionItems
                        .Any(item => item.BatchId == batchId && item.IsComplete != true);
                    if (isCompleted)
                    {
                        message = "已完成批次不可取消。";
                        return false;
                    }

                    batch.IsCancelled = true;
                    batch.CancelledBy = currentUserId;
                    batch.CancelledDate = DateTime.Now;
                    batch.CancelReason = string.IsNullOrWhiteSpace(cancelReason)
                        ? null
                        : cancelReason.Trim();

                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(CancelBatch), ex.ToString());
                message = ex.Message;
                return false;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var itemRemove = _context.InspectionBatches.FirstOrDefault(r => r.Id == id);
                    _context.InspectionBatches.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
