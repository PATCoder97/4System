using DevExpress.XtraGrid.Localization;
using System.Collections.Generic;

namespace Winform4System.Helpers
{
    internal sealed class TraditionalChineseGridLocalizer : GridLocalizer
    {
        private static readonly IReadOnlyDictionary<string, string> Captions =
            new Dictionary<string, string>
            {
                ["MenuColumnSortAscending"] = "遞增排序",
                ["MenuColumnSortDescending"] = "遞減排序",
                ["MenuColumnClearSorting"] = "清除排序",
                ["MenuColumnGroup"] = "依此欄分組",
                ["MenuColumnUnGroup"] = "取消分組",
                ["MenuColumnColumnCustomization"] = "欄位選擇器",
                ["MenuColumnBestFit"] = "最適欄寬",
                ["MenuColumnBestFitAllColumns"] = "所有欄位最適寬度",
                ["MenuColumnFilterEditor"] = "篩選編輯器",
                ["MenuColumnClearFilter"] = "清除篩選",
                ["MenuColumnShowSearchPanel"] = "顯示搜尋面板",
                ["MenuColumnHideSearchPanel"] = "隱藏搜尋面板",
                ["MenuGroupPanelClearGrouping"] = "清除分組",
                ["MenuGroupPanelHide"] = "隱藏分組面板",
                ["MenuFooterSum"] = "總和",
                ["MenuFooterMin"] = "最小值",
                ["MenuFooterMax"] = "最大值",
                ["MenuFooterCount"] = "筆數",
                ["MenuFooterAverage"] = "平均值",
                ["MenuFooterNone"] = "無"
            };

        public override string GetLocalizedString(GridStringId id)
        {
            return Captions.TryGetValue(id.ToString(), out string caption)
                ? caption
                : base.GetLocalizedString(id);
        }
    }
}
