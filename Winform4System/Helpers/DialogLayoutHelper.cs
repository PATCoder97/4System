using System;
using System.Drawing;
using System.Windows.Forms;

namespace Winform4System.Helpers
{
    internal static class DialogLayoutHelper
    {
        public static void FitToWorkingArea(Form dialog)
        {
            Rectangle area = Screen.FromControl(dialog).WorkingArea;
            int margin = Math.Max(16, (int)Math.Round(16 * dialog.DeviceDpi / 96f));
            int width = Math.Max(320, area.Width - margin * 2);
            int height = Math.Max(240, area.Height - margin * 2);
            dialog.MinimumSize = new Size(Math.Min(dialog.MinimumSize.Width, width), Math.Min(dialog.MinimumSize.Height, height));
            dialog.Size = new Size(Math.Min(dialog.Width, width), Math.Min(dialog.Height, height));
        }
    }
}
