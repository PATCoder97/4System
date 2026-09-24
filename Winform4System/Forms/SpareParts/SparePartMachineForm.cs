using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.Utils.Html.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using Winform4System.Forms.SpareParts;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartMachineForm : DevExpress.XtraEditors.XtraForm
    {
        public SparePartMachineForm()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
        }

        public SparePartFormMode eventInfo = SparePartFormMode.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = SparePartConfiguration.LoginUser.IdDepartment;

        SparePartMachine machine;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = SparePartSvgImages.Edit;
            btnDelete.ImageOptions.SvgImage = SparePartSvgImages.Remove;
            btnConfirm.ImageOptions.SvgImage = SparePartSvgImages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDisplayName.Enabled = _enable;
            txbLocation.Enabled = _enable;
            txbQuantity.Enabled = _enable;
            cbbImpLvl.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case SparePartFormMode.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case SparePartFormMode.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();

                    break;
                case SparePartFormMode.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case SparePartFormMode.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void SparePartMachineForm_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDisplayName, lcLocation, lcQuantity, lcImpLvl };
            lcImpControls = new List<LayoutControlItem>() { lcDisplayName, lcLocation, lcQuantity, lcImpLvl };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            cbbImpLvl.Properties.Items.AddRange(new string[] { "L", "M", "H" });

            switch (eventInfo)
            {
                case SparePartFormMode.Create:

                    machine = new SparePartMachine();

                    break;
                case SparePartFormMode.View:

                    machine = SparePartMachineService.Instance.GetItemById(idBase);

                    txbDisplayName.EditValue = machine.DisplayName;
                    txbLocation.EditValue = machine.Location;
                    txbQuantity.EditValue = machine.Quantity;
                    cbbImpLvl.EditValue = machine.ImpLevel;

                    break;
                case SparePartFormMode.Update:
                    break;
                case SparePartFormMode.Delete:
                    break;
                case SparePartFormMode.ViewOnly:
                    break;
                default:
                    break;
            }

            LockControl();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = SparePartFormMode.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SparePartMessage.MsgConfirmDel();

            eventInfo = SparePartFormMode.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                SparePartMessage.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var displayName = txbDisplayName.EditValue?.ToString();
            var location = txbLocation.EditValue?.ToString();
            var quantity = Convert.ToInt16(txbQuantity.EditValue);
            var impLevel = cbbImpLvl.EditValue?.ToString(); ;

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                machine.IdDept = idDeptGetData;
                machine.DisplayName = displayName;
                machine.Location = location;
                machine.Quantity = quantity;
                machine.ImpLevel = impLevel;

                switch (eventInfo)
                {
                    case SparePartFormMode.Create:

                        result = SparePartMachineService.Instance.Add(machine);

                        break;
                    case SparePartFormMode.Update:

                        result = SparePartMachineService.Instance.AddOrUpdate(machine);

                        break;
                    case SparePartFormMode.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}:\r\n{machine.DisplayName}", SparePartConfiguration.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = SparePartMachineService.Instance.RemoveById(machine.Id);

                        SparePartMachineMaterialService.Instance.RemoveByIdMachine(machine.Id);

                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                SparePartMessage.MsgErrorDB();
            }
        }
    }
}
