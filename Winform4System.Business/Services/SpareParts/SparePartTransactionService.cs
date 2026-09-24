using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Entities.SpareParts;

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
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    AddToContext(_context, item);
                    int affectedRecords = _context.SaveChanges();
                    transaction.Commit();
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
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    if (items == null || items.Count == 0)
                        return false;

                    foreach (var item in items)
                        AddToContext(_context, item);

                    int affectedRecords = _context.SaveChanges();
                    transaction.Commit();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        internal static void AddToContext(SparePartDbContext context, SparePartTransaction item)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            string transactionType = (item.TransactionType ?? string.Empty).Trim().ToLowerInvariant();
            if (transactionType != "in"
                && transactionType != "out"
                && transactionType != "transfer"
                && transactionType != "check")
            {
                throw new InvalidOperationException("備品交易類型無效。");
            }

            if (item.StorageId != 1 && item.StorageId != 2)
                throw new InvalidOperationException("備品倉庫無效。");

            var material = context.Materials.FirstOrDefault(candidate => candidate.Id == item.MaterialId);
            if (material == null)
                throw new InvalidOperationException("找不到需要更新庫存的物料。");

            double previousQuantity = item.StorageId == 1
                ? material.QuantityInMachine
                : material.QuantityInStorage;
            double updatedQuantity;
            double savedQuantity;

            switch (transactionType)
            {
                case "in":
                    EnsurePositiveQuantity(item.Quantity);
                    updatedQuantity = previousQuantity + item.Quantity;
                    savedQuantity = item.Quantity;
                    break;

                case "out":
                    EnsurePositiveQuantity(item.Quantity);
                    updatedQuantity = previousQuantity - item.Quantity;
                    if (updatedQuantity < 0)
                        throw new InvalidOperationException("領用數量大於目前庫存數量。");
                    savedQuantity = -item.Quantity;
                    break;

                case "transfer":
                    if (item.Quantity == 0)
                        throw new InvalidOperationException("轉庫數量不得為零。");
                    updatedQuantity = previousQuantity + item.Quantity;
                    if (updatedQuantity < 0)
                        throw new InvalidOperationException("轉庫數量大於目前庫存數量。");
                    savedQuantity = item.Quantity;
                    break;

                default:
                    if (item.Quantity < 0)
                        throw new InvalidOperationException("盤點數量不得小於零。");
                    updatedQuantity = item.Quantity;
                    savedQuantity = updatedQuantity - previousQuantity;
                    break;
            }

            if (item.StorageId == 1)
                material.QuantityInMachine = updatedQuantity;
            else
                material.QuantityInStorage = updatedQuantity;

            item.TransactionType = transactionType;
            item.Quantity = savedQuantity;
            item.AftQuantity = updatedQuantity;
            item.TotalQuantity = material.QuantityInMachine + material.QuantityInStorage;
            context.Transactions.Add(item);
        }

        private static void EnsurePositiveQuantity(double quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("交易數量需大於零。");
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
