using Winform4System.Business.Services.SpareParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Entities.SpareParts;
using System.Data.Entity.Migrations;

namespace Winform4System.Business.Services.SpareParts
{
    public class SparePartUserService
    {
        SparePartLogger logger;

        private static SparePartUserService instance;

        public static SparePartUserService Instance
        {
            get { if (instance == null) instance = new SparePartUserService(); return instance; }
            private set { instance = value; }
        }

        private SparePartUserService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartUser> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng bằng ký tự bắt đầu của bộ phận
        /// </summary>
        /// <param name="_idDept"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<SparePartUser> GetListByDept(string _idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Users.Where(r => r.IdDepartment.StartsWith(_idDept)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public SparePartUser GetItemById(string _UID)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Users.FirstOrDefault(r => r.Id == _UID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public SparePartUser CheckLogin(string _UID, string _pass)
        {
            // 備品備件模組使用主系統的登入階段，不沿用舊模組密碼。
            return null;
        }

        public bool Add(SparePartUser _user)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Users.Add(_user);
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

        public bool AddOrUpdate(SparePartUser _user)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Users.AddOrUpdate(_user);
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

        public bool Remove(string _idUser)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var _userDel = _context.Users.FirstOrDefault(r => r.Id == _idUser);
                    _context.Users.Remove(_userDel);

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

        public bool ChangePass(string _idUser, string _newPass)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var _userUpdate = _context.Users.FirstOrDefault(r => r.Id == _idUser);
                    _userUpdate.SecondaryPassword = _newPass;

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
