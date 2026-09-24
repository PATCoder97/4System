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
    public class SparePartMaterialService
    {
        SparePartLogger logger;

        private static SparePartMaterialService instance;

        public static SparePartMaterialService Instance
        {
            get { if (instance == null) instance = new SparePartMaterialService(); return instance; }
            private set { instance = value; }
        }

        private SparePartMaterialService() { logger = new SparePartLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<SparePartMaterial> GetAll()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetList()
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.Where(r => r.DelTime == null).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetDisabledListByStartIdDept(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials
                        .Where(r => r.IdDept.StartsWith(idDept) && r.DelTime == null && r.IsDisable == true)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetListByIds(List<int> ids)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.Where(r => ids.Contains(r.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetAllByStartIdDept(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.Where(r => r.IdDept.StartsWith(idDept)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetListByStartIdDept(string idDept)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.Where(r => r.IdDept.StartsWith(idDept) && r.DelTime == null).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public SparePartMaterial GetItemById(int id)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<SparePartMaterial> GetReplacementCandidateList(int materialId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var source = _context.Materials.FirstOrDefault(r => r.Id == materialId && r.DelTime == null);
                    if (source == null)
                    {
                        return new List<SparePartMaterial>();
                    }

                    int? currentReplacementId = source.ReplacementMaterialId;
                    var reservedTargetIds = _context.Materials
                        .Where(r => r.DelTime == null && r.ReplacementMaterialId != null && r.Id != materialId)
                        .Select(r => r.ReplacementMaterialId.Value)
                        .ToList();

                    return _context.Materials
                        .Where(r => r.DelTime == null
                            && r.IsDisable != true
                            && r.IdDept == source.IdDept
                            && r.Id != materialId)
                        .ToList()
                        .Where(r => !reservedTargetIds.Contains(r.Id) || r.Id == currentReplacementId)
                        .OrderBy(r => r.Code)
                        .ThenBy(r => r.DisplayName)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new List<SparePartMaterial>();
            }
        }

        public SparePartMaterial GetReplacementSource(int replacementMaterialId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    return _context.Materials
                        .FirstOrDefault(r => r.DelTime == null && r.ReplacementMaterialId == replacementMaterialId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        public string GetReplacementTargetUsageMessage(int materialId)
        {
            var source = GetReplacementSource(materialId);
            if (source == null)
            {
                return null;
            }

            return $"此物料目前被「{source.Code}」設定為替代料，請先清除替代設定。";
        }

        public string UpdateReplacement(int materialId, int? replacementMaterialId, DateTime? replacementDate)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var source = _context.Materials.FirstOrDefault(r => r.Id == materialId && r.DelTime == null);
                    if (source == null)
                    {
                        return "找不到對應的物料。";
                    }

                    if (source.IsDisable == true)
                    {
                        return "停用中的物料不可調整替代料。";
                    }

                    if (replacementMaterialId == null)
                    {
                        if (source.ReplacementMaterialId == null && source.ReplacementDate == null)
                        {
                            return null;
                        }

                        source.ReplacementMaterialId = null;
                        source.ReplacementDate = null;
                        _context.SaveChanges();
                        return null;
                    }

                    string validateMessage = ValidateReplacement(_context, source, replacementMaterialId, replacementDate);
                    if (!string.IsNullOrEmpty(validateMessage))
                    {
                        return validateMessage;
                    }

                    DateTime effectiveDate = replacementDate.Value.Date;
                    if (source.ReplacementMaterialId == replacementMaterialId && source.ReplacementDate == effectiveDate)
                    {
                        return null;
                    }

                    source.ReplacementMaterialId = replacementMaterialId;
                    source.ReplacementDate = effectiveDate;

                    _context.SaveChanges();
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return "更新替代料失敗。";
            }
        }

        public List<SparePartReplacementChainNode> GetReplacementChain(int materialId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var source = _context.Materials.FirstOrDefault(r => r.Id == materialId && r.DelTime == null);
                    if (source == null)
                    {
                        return new List<SparePartReplacementChainNode>();
                    }

                    var materials = _context.Materials
                        .Where(r => r.DelTime == null && r.IdDept == source.IdDept)
                        .ToList();

                    var materialById = materials.ToDictionary(r => r.Id);
                    var sourceByReplacementId = materials
                        .Where(r => r.ReplacementMaterialId != null)
                        .GroupBy(r => r.ReplacementMaterialId.Value)
                        .ToDictionary(r => r.Key, r => r.FirstOrDefault());

                    var backwardVisited = new HashSet<int>();
                    var startMaterial = source;
                    while (backwardVisited.Add(startMaterial.Id))
                    {
                        if (!sourceByReplacementId.TryGetValue(startMaterial.Id, out SparePartMaterial previousMaterial) || previousMaterial == null)
                        {
                            break;
                        }

                        startMaterial = previousMaterial;
                    }

                    var chain = new List<SparePartReplacementChainNode>();
                    var forwardVisited = new HashSet<int>();
                    var currentMaterial = startMaterial;
                    int step = 1;

                    while (currentMaterial != null && forwardVisited.Add(currentMaterial.Id))
                    {
                        SparePartMaterial nextMaterial = null;
                        if (currentMaterial.ReplacementMaterialId != null)
                        {
                            materialById.TryGetValue(currentMaterial.ReplacementMaterialId.Value, out nextMaterial);
                        }

                        chain.Add(new SparePartReplacementChainNode
                        {
                            StepNo = step++,
                            MaterialId = currentMaterial.Id,
                            Code = currentMaterial.Code,
                            DisplayName = currentMaterial.DisplayName,
                            QuantityInStorage = currentMaterial.QuantityInStorage,
                            QuantityInMachine = currentMaterial.QuantityInMachine,
                            TotalQuantity = currentMaterial.QuantityInStorage + currentMaterial.QuantityInMachine,
                            Price = currentMaterial.Price,
                            ReplacementMaterialId = currentMaterial.ReplacementMaterialId,
                            ReplacementMaterialCode = nextMaterial?.Code,
                            ReplacementDate = currentMaterial.ReplacementDate,
                            Status = currentMaterial.IsDisable == true ? "停用" : "啟用"
                        });

                        currentMaterial = nextMaterial;
                    }

                    return chain;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new List<SparePartReplacementChainNode>();
            }
        }

        private string ValidateReplacement(SparePartDbContext context, SparePartMaterial source, int? replacementMaterialId, DateTime? replacementDate)
        {
            if (replacementMaterialId == null)
            {
                return null;
            }

            if (replacementDate == null)
            {
                return "請選擇替代日期。";
            }

            if (replacementMaterialId.Value == source.Id)
            {
                return "不可將物料設定為自己的替代料。";
            }

            var target = context.Materials.FirstOrDefault(r => r.Id == replacementMaterialId.Value && r.DelTime == null);
            if (target == null)
            {
                return "找不到對應的替代物料。";
            }

            if (target.IsDisable == true)
            {
                return "不可選擇停用中的物料作為替代料。";
            }

            if (target.IdDept != source.IdDept)
            {
                return "替代料必須為同課物料。";
            }

            var replacementSource = context.Materials.FirstOrDefault(r =>
                r.DelTime == null &&
                r.ReplacementMaterialId == replacementMaterialId.Value &&
                r.Id != source.Id);
            if (replacementSource != null)
            {
                return $"物料「{target.Code}」已被「{replacementSource.Code}」設定為替代料。";
            }

            if (WillCreateReplacementLoop(context, source.Id, replacementMaterialId.Value))
            {
                return "此設定會形成替代循環，請重新選擇。";
            }

            return null;
        }

        private bool WillCreateReplacementLoop(SparePartDbContext context, int sourceId, int replacementMaterialId)
        {
            var visitedIds = new HashSet<int> { sourceId };
            int? currentId = replacementMaterialId;

            while (currentId != null)
            {
                if (currentId.Value == sourceId)
                {
                    return true;
                }

                if (!visitedIds.Add(currentId.Value))
                {
                    return true;
                }

                currentId = context.Materials
                    .Where(r => r.Id == currentId.Value && r.DelTime == null)
                    .Select(r => r.ReplacementMaterialId)
                    .FirstOrDefault();
            }

            return false;
        }

        public int Add(SparePartMaterial item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Materials.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                    {
                        return item.Id;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<SparePartMaterial> items)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Materials.AddRange(items);
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

        public bool AddOrUpdate(SparePartMaterial item)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    _context.Materials.AddOrUpdate(item);
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

        public bool RemoveById(int id, string userDel)
        {
            try
            {
                Winform4System.Core.Security.CurrentAuthorization.Demand("ASSET.SPARE_PART.DELETE");
                using (var _context = new SparePartDbContext())
                {
                    var itemRemove = _context.Materials.FirstOrDefault(r => r.Id == id);
                    if (itemRemove == null) return false;

                    if (_context.Materials.Any(r => r.DelTime == null && r.ReplacementMaterialId == id))
                    {
                        return false;
                    }

                    itemRemove.DelTime = DateTime.Now;
                    itemRemove.UserDel = userDel;

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

        public bool DisableById(int id, string userId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var item = _context.Materials.FirstOrDefault(r => r.Id == id && r.DelTime == null);
                    if (item == null) return false;

                    if (_context.Materials.Any(r => r.DelTime == null && r.ReplacementMaterialId == id))
                    {
                        return false;
                    }

                    item.IsDisable = true;
                    item.DisabledBy = userId;
                    item.DisabledDate = DateTime.Now;
                    item.EnabledBy = null;
                    item.EnabledDate = null;

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

        public bool EnableById(int id, string userId)
        {
            try
            {
                using (var _context = new SparePartDbContext())
                {
                    var item = _context.Materials.FirstOrDefault(r => r.Id == id && r.DelTime == null);
                    if (item == null) return false;

                    item.IsDisable = false;
                    item.EnabledBy = userId;
                    item.EnabledDate = DateTime.Now;

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

    public class SparePartReplacementChainNode
    {
        public int StepNo { get; set; }
        public int MaterialId { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public double QuantityInStorage { get; set; }
        public double QuantityInMachine { get; set; }
        public double TotalQuantity { get; set; }
        public int Price { get; set; }
        public int? ReplacementMaterialId { get; set; }
        public string ReplacementMaterialCode { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string Status { get; set; }
    }
}
