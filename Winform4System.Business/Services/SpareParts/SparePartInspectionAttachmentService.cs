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
    public class SparePartInspectionAttachmentService
    {
        SparePartLogger logger;

        private static SparePartInspectionAttachmentService instance;

        public static SparePartInspectionAttachmentService Instance
        {
            get { if (instance == null) instance = new SparePartInspectionAttachmentService(); return instance; }
            private set { instance = value; }
        }

        private SparePartInspectionAttachmentService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        /// <summary>
        /// Thêm phụ kiện và trả về Id của phụ kiện vừa thêm
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>IdAttachment</returns>
        public int Add(SparePartInspectionAttachment attachment)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionAttachments.Add(attachment);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return attachment.Id;
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

        public int AddOrUpdate(SparePartInspectionAttachment attachment)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.InspectionAttachments.AddOrUpdate(attachment);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return attachment.Id;
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

        /// <summary>
        /// Lấy các phụ kiện theo thread: 301, 302,...
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public List<SparePartInspectionAttachment> GetListByThread(string thread)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionAttachments.Where(r => r.Thread == thread).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartInspectionAttachment> GetListByThreads(List<string> threads)
        {
            try
            {
                if (threads == null)
                {
                    return new List<SparePartInspectionAttachment>();
                }

                threads = threads
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct()
                    .ToList();

                if (threads.Count == 0)
                {
                    return new List<SparePartInspectionAttachment>();
                }

                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionAttachments.Where(r => threads.Contains(r.Thread)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy được các phụ kiện bằng danh sách id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SparePartInspectionAttachment> GetListById(List<int> ids)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionAttachments.Where(r => ids.Contains(r.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartInspectionAttachment GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.InspectionAttachments.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var itemsRemove = _context.InspectionAttachments.FirstOrDefault(r => r.Id == id);
                    _context.InspectionAttachments.Remove(itemsRemove);

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

