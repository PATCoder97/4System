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
    public class SparePartInspectionItemService
    {
        SparePartLogger logger;

        private static SparePartInspectionItemService instance;

        public static SparePartInspectionItemService Instance
        {
            get { if (instance == null) instance = new SparePartInspectionItemService(); return instance; }
            private set { instance = value; }
        }

        private SparePartInspectionItemService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartInspectionItem> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionItems.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartInspectionItem> GetListRechecking()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionItems
                        .Join(_context.InspectionBatches.Where(batch => !batch.IsCancelled),
                              material => material.BatchId,
                              batch => batch.Id,
                              (material, batch) => material)
                        .Where(item => item.IsComplete != true)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartInspectionItem GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionItems.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartInspectionItem item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionItems.Add(item);
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

        public bool AddRange(List<SparePartInspectionItem> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionItems.AddRange(items);
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

        public bool AddOrUpdate(SparePartInspectionItem item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionItems.AddOrUpdate(item);
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

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var itemRemove = _context.InspectionItems.FirstOrDefault(r => r.Id == id);
                    _context.InspectionItems.Remove(itemRemove);

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

