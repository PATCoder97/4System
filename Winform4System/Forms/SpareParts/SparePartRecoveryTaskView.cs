using System.Windows.Forms;

namespace Winform4System.Forms.SpareParts
{
    public class SparePartRecoveryTaskView : SparePartRecoveryView
    {
        public SparePartRecoveryTaskView()
        {
            Name = nameof(SparePartRecoveryTaskView);
            Dock = DockStyle.Fill;
        }

        protected override bool IsTaskView => true;
    }
}
