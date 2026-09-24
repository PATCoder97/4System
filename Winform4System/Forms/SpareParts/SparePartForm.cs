using System;
using Winform4System.Core.Models;
using Winform4System.Forms.Common;
using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public sealed class SparePartForm : FluentModuleForm
    {
        public SparePartForm(UserSession session)
            : base("備品備件管理", "sparePart")
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            SparePartConfiguration.Initialize(session);
            BuildNavigation();
            Shown += (sender, args) => OpenTab("materials", "備品資料", () => new SparePartMaterialView());
        }

        private void BuildNavigation()
        {
            var warehouseGroup = AddNavigationGroup("機邊庫");
            AddNavigationItem(warehouseGroup, "materials", "備品資料", SvgIconCatalog.View, () => new SparePartMaterialView());
            AddNavigationItem(warehouseGroup, "machines", "設備管理", SvgIconCatalog.Equipment, () => new SparePartMachineView());
            AddNavigationItem(warehouseGroup, "transactions", "進出庫管理", SvgIconCatalog.Transfer, () => new SparePartTransactionView());
            AddNavigationItem(warehouseGroup, "recheck", "複盤作業", SvgIconCatalog.Refresh, () => new SparePartRecheckView());
            AddNavigationItem(warehouseGroup, "my-recovery", "我的回收作業", SvgIconCatalog.UserTransfer, () => new SparePartRecoveryTaskView());

            if (!SparePartHelper.CanManageAllDepartments())
                return;

            var managerGroup = AddNavigationGroup("機邊庫【經理室】", "經理室的功能");
            AddNavigationItem(managerGroup, "inspection", "盤點批次", SvgIconCatalog.SelectionChecked, () => new SparePartInspectionView());
            AddNavigationItem(managerGroup, "cost", "成本計算", SvgIconCatalog.Cost, () => new SparePartCostView());
            AddNavigationItem(managerGroup, "recovery", "回收管理", SvgIconCatalog.Schedule, () => new SparePartRecoveryView());
        }
    }
}
