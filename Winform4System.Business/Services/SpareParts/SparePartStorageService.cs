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
    public class SparePartStorageService
    {
        SparePartLogger logger;

        private static SparePartStorageService instance;

        public static SparePartStorageService Instance
        {
            get { if (instance == null) instance = new SparePartStorageService(); return instance; }
            private set { instance = value; }
        }

        private SparePartStorageService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartStorage> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Storages.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartStorage GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Storages.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartStorage item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Storages.Add(item);
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

        public bool AddRange(List<SparePartStorage> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Storages.AddRange(items);
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

        public bool AddOrUpdate(SparePartStorage item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Storages.AddOrUpdate(item);
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
                    var itemRemove = _context.Storages.FirstOrDefault(r => r.Id == id);
                    _context.Storages.Remove(itemRemove);

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

