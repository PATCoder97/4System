using Winform4System.DataAccess;
using Winform4System.DataAccess.Entities.SpareParts;
using Winform4System.Business.Services.SpareParts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Winform4System.Business.Services.SpareParts
{
    public class SparePartDepartmentService
    {
        SparePartLogger logger;

        private static SparePartDepartmentService instance;

        public static SparePartDepartmentService Instance
        {
            get { if (instance == null) instance = new SparePartDepartmentService(); return instance; }
            private set { instance = value; }
        }

        private SparePartDepartmentService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartDepartment> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Departments.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartDepartment> GetListByParent(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Departments.Where(d => d.IdParent == _context.Departments
                                      .Where(p => p.Id == idDept)
                                      .Select(p => p.IdChild)
                                      .FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartDepartment> GetAllChildren(int idChildDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var result = new List<SparePartDepartment>();

                    // Hàm đệ quy nội bộ
                    void CollectChildren(int parentId)
                    {
                        var children = _context.Departments
                                               .Where(d => d.IdParent == parentId)
                                               .ToList();

                        foreach (var child in children)
                        {
                            result.Add(child);
                            // Gọi tiếp cho cấp con của child
                            if (child.IdChild != null)
                                CollectChildren((int)child.IdChild);
                        }
                    }

                    // Thêm chính nó vào trước
                    var root = _context.Departments
                                       .FirstOrDefault(d => d.IdChild == idChildDept);
                    if (root != null)
                    {
                        result.Add(root);
                        CollectChildren(idChildDept);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }


        public SparePartDepartment GetItemById(string _idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Departments.FirstOrDefault(r => r.Id == _idDept);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartDepartment GetItemByParentId(int _idParent)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Departments.FirstOrDefault(r => r.IdParent == _idParent);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(SparePartDepartment _dept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Departments.Add(_dept);
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

        public bool AddOrUpdate(SparePartDepartment _dept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Departments.AddOrUpdate(_dept);
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

        public bool Remove(string _idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var _itemDel = _context.Departments.FirstOrDefault(r => r.Id == _idDept);
                    _context.Departments.Remove(_itemDel);

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

