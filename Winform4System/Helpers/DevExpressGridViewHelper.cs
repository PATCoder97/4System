using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Windows.Forms;

namespace Winform4System.Helpers
{
    public static class DevExpressGridViewHelper
    {
        public static void ReadOnlyGridView(this GridView gridView_, bool active = true)
        {
            gridView_.OptionsEditForm.ShowOnDoubleClick = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnEnterKey = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnF2Key = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsBehavior.EditingMode = active ? GridEditingMode.EditFormInplace : GridEditingMode.Inplace;
        }

        public static void CopyFocusedCellOnCtrlC(object sender, KeyEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn) != null && view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).ToString() != String.Empty)
                    Clipboard.SetText(view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).ToString());
                e.Handled = true;
            }
        }

    }
}
