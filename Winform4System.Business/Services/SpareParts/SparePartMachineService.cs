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
    public class SparePartMachineService
    {
        SparePartLogger logger;

        private static SparePartMachineService instance;

        public static SparePartMachineService Instance
        {
            get { if (instance == null) instance = new SparePartMachineService(); return instance; }
            private set { instance = value; }
        }

        private SparePartMachineService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartMachine> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Machines.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMachine> GetListByIdDept(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Machines.Where(r => r.IdDept == idDept).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMachine> GetListByStartIdDept(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Machines.Where(r => r.IdDept.StartsWith(idDept)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMachine> GetListByIds(List<int> ids)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Machines.Where(r => ids.Contains(r.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartMachine GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Machines.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartMachine item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Machines.Add(item);
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

        public bool AddRange(List<SparePartMachine> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Machines.AddRange(items);
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

        public bool AddOrUpdate(SparePartMachine item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Machines.AddOrUpdate(item);
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
                    var itemRemove = _context.Machines.FirstOrDefault(r => r.Id == id);
                    _context.Machines.Remove(itemRemove);

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

