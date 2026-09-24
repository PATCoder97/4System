using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Winform4System.Forms.SpareParts;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartRecoveryGuideForm : XtraForm
    {
        private readonly BindingSource sourceGuides = new BindingSource();
        private SparePartRefreshHelper helper;

        public SparePartRecoveryGuideForm()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            ConfigureGrid();

            helper = new SparePartRefreshHelper(gvData, "Id");
            gcData.DataSource = sourceGuides;
        }

        private void SparePartRecoveryGuideForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void InitializeIcon()
        {
            btnUpload.ImageOptions.SvgImage = SparePartSvgImages.UploadFile;
            btnView.ImageOptions.SvgImage = SparePartSvgImages.View;
            btnDelete.ImageOptions.SvgImage = SparePartSvgImages.Remove;
            btnReload.ImageOptions.SvgImage = SparePartSvgImages.Reload;
        }

        private void ConfigureGrid()
        {
            gvData.Columns.Clear();
            gvData.ReadOnlyGridView();
            gvData.OptionsBehavior.Editable = false;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowAutoFilterRow = true;
            gvData.OptionsView.ShowGroupPanel = false;
            gvData.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;

            gvData.Columns.AddVisible(nameof(SparePartRecoveryGuide.Title), "\u6a19\u984c").Width = 220;
            gvData.Columns.AddVisible(nameof(SparePartRecoveryGuide.ActualName), "\u6a94\u540d").Width = 260;
            gvData.Columns.AddVisible(nameof(SparePartRecoveryGuide.DisplayOrder), "\u6392\u5e8f").Width = 80;
            gvData.Columns.AddVisible(nameof(SparePartRecoveryGuide.UploadedBy), "\u4e0a\u50b3\u8005").Width = 120;

            var colDate = gvData.Columns.AddVisible(nameof(SparePartRecoveryGuide.UploadedDate), "\u4e0a\u50b3\u6642\u9593");
            colDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDate.Width = 160;
        }

        private void LoadData()
        {
            sourceGuides.DataSource = SparePartRecoveryService.Instance.GetGuideList();
            gvData.BestFitColumns();
        }

        private SparePartRecoveryGuide GetFocusedGuide()
        {
            if (gvData.FocusedRowHandle < 0)
            {
                return null;
            }

            return gvData.GetRow(gvData.FocusedRowHandle) as SparePartRecoveryGuide;
        }

        private void btnUpload_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = SparePartConfiguration.FilterFile;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string file in dialog.FileNames)
                {
                    var saved = SparePartRecoveryFileHelper.SaveGuideFile(file);
                    int id = SparePartRecoveryService.Instance.AddGuide(new SparePartRecoveryGuide
                    {
                        Title = Path.GetFileNameWithoutExtension(saved.actualName),
                        ActualName = saved.actualName,
                        EncryptionName = saved.encryptionName,
                        FileExt = saved.extension,
                        UploadedBy = SparePartConfiguration.LoginUser.Id,
                        UploadedDate = DateTime.Now
                    });

                    if (id <= 0)
                    {
                        SparePartMessage.MsgError($"\u4e0a\u50b3\u5931\u6557: {Path.GetFileName(file)}");
                    }
                }
            }

            LoadData();
        }

        private void btnView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var guide = GetFocusedGuide();
            if (guide == null)
            {
                return;
            }

            SparePartRecoveryFileHelper.OpenGuideFiles(new[] { guide });
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var guide = GetFocusedGuide();
            if (guide == null)
            {
                return;
            }

            var result = XtraMessageBox.Show(
                $"\u78ba\u5b9a\u8981\u522a\u9664\u6307\u5f15\uff1a\r\n{guide.Title}",
                SparePartConfiguration.SoftNameTW,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            if (!SparePartRecoveryService.Instance.DeactivateGuide(guide.Id))
            {
                SparePartMessage.MsgErrorDB();
                return;
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
