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
    public class SparePartMachineMaterialService
    {
        SparePartLogger logger;

        private static SparePartMachineMaterialService instance;

        public static SparePartMachineMaterialService Instance
        {
            get { if (instance == null) instance = new SparePartMachineMaterialService(); return instance; }
            private set { instance = value; }
        }

        private SparePartMachineMaterialService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartMachineMaterial> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MachineMaterials.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMachineMaterial> GetListByIdMaterial(int idMaterial)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MachineMaterials.Where(r => r.MaterialId == idMaterial).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMachineMaterial> GetListByIdMachine(int idMachine)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MachineMaterials.Where(r => r.MachineId == idMachine).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartMachineMaterial GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MachineMaterials.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartMachineMaterial item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.MachineMaterials.Add(item);
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

        public bool AddRange(List<SparePartMachineMaterial> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.MachineMaterials.AddRange(items);
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

        public bool AddOrUpdate(SparePartMachineMaterial item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.MachineMaterials.AddOrUpdate(item);
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
                    var itemRemove = _context.MachineMaterials.FirstOrDefault(r => r.Id == id);
                    _context.MachineMaterials.Remove(itemRemove);

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

        public bool RemoveByIdMaterial(int idMachine)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var itemRemove = _context.MachineMaterials.Where(r => r.MaterialId == idMachine);
                    _context.MachineMaterials.RemoveRange(itemRemove);

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

        public bool RemoveByIdMachine(int idMachine)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var itemRemove = _context.MachineMaterials.Where(r => r.MachineId == idMachine);
                    _context.MachineMaterials.RemoveRange(itemRemove);

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

