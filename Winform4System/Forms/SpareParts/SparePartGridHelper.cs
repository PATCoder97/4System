using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winform4System.Forms.SpareParts
{
    public static class SparePartGridHelper
    {
        public static void ReadOnlyGridView(this GridView gridView_, bool active = true)
        {
            gridView_.OptionsEditForm.ShowOnDoubleClick = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnEnterKey = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnF2Key = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsBehavior.EditingMode = active ? GridEditingMode.EditFormInplace : GridEditingMode.Inplace;
        }

        public static void GridViewCopyCellData_KeyDown(object sender, KeyEventArgs e)
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
