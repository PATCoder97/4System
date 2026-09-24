using DevExpress.XtraTreeList.Localization;

namespace Winform4System.Helpers
{
    internal sealed class TraditionalChineseTreeListLocalizer : TreeListLocalizer
    {
        public override string GetLocalizedString(TreeListStringId id)
        {
            switch (id)
            {
                case TreeListStringId.MenuFooterSum:
                    return "總和";
                case TreeListStringId.MenuFooterMin:
                    return "最小值";
                case TreeListStringId.MenuFooterMax:
                    return "最大值";
                case TreeListStringId.MenuFooterCount:
                    return "計數";
                case TreeListStringId.MenuFooterAverage:
                    return "平均值";
                case TreeListStringId.MenuFooterNone:
                    return "無";
                case TreeListStringId.MenuFooterAllNodes:
                    return "所有節點";
                case TreeListStringId.MenuFooterSumFormat:
                    return "總和={0:#.##}";
                case TreeListStringId.MenuFooterMinFormat:
                    return "最小值={0}";
                case TreeListStringId.MenuFooterMaxFormat:
                    return "最大值={0}";
                case TreeListStringId.MenuFooterCountFormat:
                    return "{0}";
                case TreeListStringId.MenuFooterAverageFormat:
                    return "平均值={0:#.##}";
                case TreeListStringId.MenuColumnFooterHide:
                    return "隱藏摘要列";
                case TreeListStringId.MenuColumnFooterShow:
                    return "顯示摘要列";
                case TreeListStringId.MenuColumnSortAscending:
                    return "升冪排序";
                case TreeListStringId.MenuColumnSortDescending:
                    return "降冪排序";
                case TreeListStringId.MenuColumnClearSorting:
                    return "清除排序";
                case TreeListStringId.MenuColumnColumnCustomization:
                    return "欄位選擇器";
                case TreeListStringId.MenuColumnBestFit:
                    return "最佳欄寬";
                case TreeListStringId.MenuColumnBestFitAllColumns:
                    return "所有欄位最佳寬度";
                case TreeListStringId.MenuColumnAutoFilterRowHide:
                    return "隱藏自動篩選列";
                case TreeListStringId.MenuColumnAutoFilterRowShow:
                    return "顯示自動篩選列";
                case TreeListStringId.MenuColumnFilterEditor:
                    return "篩選編輯器...";
                case TreeListStringId.MenuColumnClearFilter:
                    return "清除篩選";
                case TreeListStringId.MenuColumnFindFilterHide:
                    return "隱藏搜尋面板";
                case TreeListStringId.MenuColumnFindFilterShow:
                    return "顯示搜尋面板";
                case TreeListStringId.MenuColumnExpressionEditor:
                    return "運算式編輯器...";
                case TreeListStringId.MenuColumnBandCustomization:
                    return "欄位／群組選擇器";
                case TreeListStringId.MenuColumnConditionalFormatting:
                    return "條件式格式設定";
                case TreeListStringId.MenuColumnClearSortingAllColumns:
                    return "清除所有排序";
                case TreeListStringId.MenuNodeExpandAll:
                    return "全部展開";
                case TreeListStringId.MenuNodeCollapseAll:
                    return "全部收合";
                case TreeListStringId.MenuNodeExpand:
                    return "展開";
                case TreeListStringId.MenuNodeCollapse:
                    return "收合";
                case TreeListStringId.MenuNodeAddNode:
                    return "新增節點";
                case TreeListStringId.MenuNodeAddChildNode:
                    return "新增子節點";
                default:
                    return base.GetLocalizedString(id);
            }
        }
    }
}
