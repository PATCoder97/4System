using Winform4System.DataAccess;
using Winform4System.DataAccess.Entities.SpareParts;
using Winform4System.Business.Services.SpareParts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace Winform4System.Business.Services.SpareParts
{
    public class SparePartMaterialPhotoService
    {
        SparePartLogger logger;

        private static SparePartMaterialPhotoService instance;

        public static SparePartMaterialPhotoService Instance
        {
            get { if (instance == null) instance = new SparePartMaterialPhotoService(); return instance; }
            private set { instance = value; }
        }

        private SparePartMaterialPhotoService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartMaterialPhoto> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MaterialPhotos.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterialPhoto> GetListByMaterialId(int materialId, bool activeOnly = false)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var query = _context.MaterialPhotos.Where(r => r.MaterialId == materialId);
                    if (activeOnly)
                    {
                        query = query.Where(r => r.IsActive);
                    }

                    return query.OrderByDescending(r => r.UploadedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartMaterialPhoto GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MaterialPhotos.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartMaterialPhoto GetActivePhoto(int materialId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.MaterialPhotos.FirstOrDefault(r => r.MaterialId == materialId && r.IsActive);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(SparePartMaterialPhoto item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.MaterialPhotos.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddOrUpdate(SparePartMaterialPhoto item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.MaterialPhotos.AddOrUpdate(item);
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

        public int AddOrReplace(SparePartMaterialPhoto item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var oldItems = _context.MaterialPhotos.Where(r => r.MaterialId == item.MaterialId && r.IsActive).ToList();

                    foreach (var oldItem in oldItems)
                    {
                        oldItem.IsActive = false;
                    }

                    item.IsActive = true;
                    _context.MaterialPhotos.Add(item);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool DeactivateById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var item = _context.MaterialPhotos.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;

                    item.IsActive = false;
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
    }
}

