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
    public class SparePartTransactionService
    {
        SparePartLogger logger;

        private static SparePartTransactionService instance;

        public static SparePartTransactionService Instance
        {
            get { if (instance == null) instance = new SparePartTransactionService(); return instance; }
            private set { instance = value; }
        }

        private SparePartTransactionService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartTransaction> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Transactions.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartTransaction> GetListByDate(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Transactions.Where(r => r.CreatedDate >= dateFrom && r.CreatedDate <= dateTo).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartTransaction> GetUnnotifiedTransactions()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Transactions.Where(r => r.NotifyDate == null).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartTransaction> GetListByidMaterial(int idMaterial)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Transactions.Where(r => r.MaterialId == idMaterial).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartTransaction GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Transactions.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartTransaction item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Transactions.Add(item);
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

        public bool AddRange(List<SparePartTransaction> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Transactions.AddRange(items);
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

        public bool AddOrUpdate(SparePartTransaction item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Transactions.AddOrUpdate(item);
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
                    var itemRemove = _context.Transactions.FirstOrDefault(r => r.Id == id);
                    _context.Transactions.Remove(itemRemove);

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

