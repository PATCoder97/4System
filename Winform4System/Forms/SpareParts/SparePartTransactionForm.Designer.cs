using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Winform4System.Forms.SpareParts
{
    partial class SparePartTransactionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        ImageComboBoxEdit cbbRecoveryOption;
        SearchLookUpEdit sleAssignedUser;
        GridView gvAssignedUserLookup;
        DateEdit dePlannedDisposeDate;
        LookUpEdit cbbRestockStorage;
        MemoEdit memoRecoveryNote;
        SimpleButton btnViewGuideDocs;

        LayoutControlItem lcRecoveryOption;
        LayoutControlItem lcAssignedUser;
        LayoutControlItem lcPlannedDisposeDate;
        LayoutControlItem lcRestockStorage;
        LayoutControlItem lcRecoveryNote;
        LayoutControlItem lcGuideDocs;

        private DevExpress.XtraGrid.Columns.GridColumn colAssignedUserId, colAssignedUserName, colAssignedUserDepartment;

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SparePartTransactionForm));
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbQuantity = new DevExpress.XtraEditors.TextEdit();
            this.txbDesc = new DevExpress.XtraEditors.TextEdit();
            this.cbbStorageFrom = new DevExpress.XtraEditors.LookUpEdit();
            this.txbStorageFromQuantity = new DevExpress.XtraEditors.TextEdit();
            this.cbbStorageTo = new DevExpress.XtraEditors.LookUpEdit();
            this.txbStorageToQuantity = new DevExpress.XtraEditors.TextEdit();
            this.txbPrice = new DevExpress.XtraEditors.TextEdit();
            this.txbExpDate = new DevExpress.XtraEditors.DateEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcDesc = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStorageFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStorageTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStorageFromQuantity = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStorageToQuantity = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcQuantity = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExpDate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbQuantity.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStorageFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbStorageFromQuantity.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStorageTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbStorageToQuantity.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExpDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExpDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageFromQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageToQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExpDate)).BeginInit();
            this.SuspendLayout();
            //
            // barManagerTP
            //
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnEdit,
            this.btnDelete,
            this.btnConfirm});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 4;
            //
            // bar2
            //
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.bar2.BarAppearance.Disabled.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnEdit, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDelete, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "主選單";
            //
            // btnEdit
            //
            this.btnEdit.Caption = "編輯";
            this.btnEdit.Id = 0;
            this.btnEdit.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnEdit.ImageOptions.SvgImage")));
            this.btnEdit.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnEdit.Name = "btnEdit";
            //
            // btnDelete
            //
            this.btnDelete.Caption = "刪除";
            this.btnDelete.Id = 1;
            this.btnDelete.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnDelete.Name = "btnDelete";
            //
            // btnConfirm
            //
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 2;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(640, 49);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 285);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(640, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 236);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(398, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 236);
            //
            // layoutControl1
            //
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.txbQuantity);
            this.layoutControl1.Controls.Add(this.txbDesc);
            this.layoutControl1.Controls.Add(this.cbbStorageFrom);
            this.layoutControl1.Controls.Add(this.txbStorageFromQuantity);
            this.layoutControl1.Controls.Add(this.cbbStorageTo);
            this.layoutControl1.Controls.Add(this.txbStorageToQuantity);
            this.layoutControl1.Controls.Add(this.txbPrice);
            this.layoutControl1.Controls.Add(this.txbExpDate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(433, 0, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(640, 236);
            this.layoutControl1.TabIndex = 11;
            this.layoutControl1.Text = "layoutControl1";
            //
            // txbQuantity
            //
            this.txbQuantity.EditValue = "0";
            this.txbQuantity.Location = new System.Drawing.Point(100, 84);
            this.txbQuantity.Name = "txbQuantity";
            this.txbQuantity.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbQuantity.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbQuantity.Properties.Appearance.Options.UseFont = true;
            this.txbQuantity.Properties.Appearance.Options.UseForeColor = true;
            this.txbQuantity.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txbQuantity.Properties.MaskSettings.Set("mask", "F");
            this.txbQuantity.Properties.MaskSettings.Set("autoHideDecimalSeparator", null);
            this.txbQuantity.Properties.UseMaskAsDisplayFormat = true;
            this.txbQuantity.Size = new System.Drawing.Size(286, 32);
            this.txbQuantity.StyleController = this.layoutControl1;
            this.txbQuantity.TabIndex = 9;
            //
            // txbDesc
            //
            this.txbDesc.Location = new System.Drawing.Point(100, 120);
            this.txbDesc.Name = "txbDesc";
            this.txbDesc.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbDesc.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDesc.Properties.Appearance.Options.UseFont = true;
            this.txbDesc.Properties.Appearance.Options.UseForeColor = true;
            this.txbDesc.Size = new System.Drawing.Size(286, 32);
            this.txbDesc.StyleController = this.layoutControl1;
            this.txbDesc.TabIndex = 9;
            //
            // cbbStorageFrom
            //
            this.cbbStorageFrom.Location = new System.Drawing.Point(100, 12);
            this.cbbStorageFrom.Name = "cbbStorageFrom";
            this.cbbStorageFrom.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbStorageFrom.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbStorageFrom.Properties.Appearance.Options.UseFont = true;
            this.cbbStorageFrom.Properties.Appearance.Options.UseForeColor = true;
            this.cbbStorageFrom.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbStorageFrom.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbStorageFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbStorageFrom.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "Name1")});
            this.cbbStorageFrom.Properties.DropDownRows = 3;
            this.cbbStorageFrom.Properties.NullText = "";
            this.cbbStorageFrom.Properties.PopupSizeable = false;
            this.cbbStorageFrom.Properties.ShowHeader = false;
            this.cbbStorageFrom.Size = new System.Drawing.Size(141, 32);
            this.cbbStorageFrom.StyleController = this.layoutControl1;
            this.cbbStorageFrom.TabIndex = 10;
            this.cbbStorageFrom.EditValueChanged += new System.EventHandler(this.cbbStorageFrom_EditValueChanged);
            //
            // txbStorageFromQuantity
            //
            this.txbStorageFromQuantity.EditValue = "0";
            this.txbStorageFromQuantity.Location = new System.Drawing.Point(288, 12);
            this.txbStorageFromQuantity.Name = "txbStorageFromQuantity";
            this.txbStorageFromQuantity.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbStorageFromQuantity.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbStorageFromQuantity.Properties.Appearance.Options.UseFont = true;
            this.txbStorageFromQuantity.Properties.Appearance.Options.UseForeColor = true;
            this.txbStorageFromQuantity.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txbStorageFromQuantity.Properties.MaskSettings.Set("mask", "F");
            this.txbStorageFromQuantity.Properties.MaskSettings.Set("autoHideDecimalSeparator", null);
            this.txbStorageFromQuantity.Properties.UseMaskAsDisplayFormat = true;
            this.txbStorageFromQuantity.Size = new System.Drawing.Size(98, 32);
            this.txbStorageFromQuantity.StyleController = this.layoutControl1;
            this.txbStorageFromQuantity.TabIndex = 9;
            //
            // cbbStorageTo
            //
            this.cbbStorageTo.Location = new System.Drawing.Point(100, 48);
            this.cbbStorageTo.Name = "cbbStorageTo";
            this.cbbStorageTo.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbStorageTo.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbStorageTo.Properties.Appearance.Options.UseFont = true;
            this.cbbStorageTo.Properties.Appearance.Options.UseForeColor = true;
            this.cbbStorageTo.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbStorageTo.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbStorageTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbStorageTo.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "Name1")});
            this.cbbStorageTo.Properties.DropDownRows = 3;
            this.cbbStorageTo.Properties.NullText = "";
            this.cbbStorageTo.Properties.PopupSizeable = false;
            this.cbbStorageTo.Properties.ShowHeader = false;
            this.cbbStorageTo.Size = new System.Drawing.Size(141, 32);
            this.cbbStorageTo.StyleController = this.layoutControl1;
            this.cbbStorageTo.TabIndex = 10;
            this.cbbStorageTo.EditValueChanged += new System.EventHandler(this.cbbStorageTo_EditValueChanged);
            //
            // txbStorageToQuantity
            //
            this.txbStorageToQuantity.EditValue = "0";
            this.txbStorageToQuantity.Location = new System.Drawing.Point(288, 48);
            this.txbStorageToQuantity.Name = "txbStorageToQuantity";
            this.txbStorageToQuantity.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbStorageToQuantity.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbStorageToQuantity.Properties.Appearance.Options.UseFont = true;
            this.txbStorageToQuantity.Properties.Appearance.Options.UseForeColor = true;
            this.txbStorageToQuantity.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txbStorageToQuantity.Properties.MaskSettings.Set("mask", "F");
            this.txbStorageToQuantity.Properties.MaskSettings.Set("autoHideDecimalSeparator", null);
            this.txbStorageToQuantity.Properties.UseMaskAsDisplayFormat = true;
            this.txbStorageToQuantity.Size = new System.Drawing.Size(98, 32);
            this.txbStorageToQuantity.StyleController = this.layoutControl1;
            this.txbStorageToQuantity.TabIndex = 9;
            //
            // txbPrice
            //
            this.txbPrice.EditValue = "0";
            this.txbPrice.Location = new System.Drawing.Point(100, 156);
            this.txbPrice.Name = "txbPrice";
            this.txbPrice.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbPrice.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbPrice.Properties.Appearance.Options.UseFont = true;
            this.txbPrice.Properties.Appearance.Options.UseForeColor = true;
            this.txbPrice.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txbPrice.Properties.MaskSettings.Set("mask", "N0");
            this.txbPrice.Properties.MaskSettings.Set("autoHideDecimalSeparator", null);
            this.txbPrice.Properties.UseMaskAsDisplayFormat = true;
            this.txbPrice.Size = new System.Drawing.Size(286, 32);
            this.txbPrice.StyleController = this.layoutControl1;
            this.txbPrice.TabIndex = 9;
            //
            // txbExpDate
            //
            this.txbExpDate.EditValue = null;
            this.txbExpDate.Location = new System.Drawing.Point(100, 192);
            this.txbExpDate.Name = "txbExpDate";
            this.txbExpDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbExpDate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbExpDate.Properties.Appearance.Options.UseFont = true;
            this.txbExpDate.Properties.Appearance.Options.UseForeColor = true;
            this.txbExpDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbExpDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbExpDate.Size = new System.Drawing.Size(286, 32);
            this.txbExpDate.StyleController = this.layoutControl1;
            this.txbExpDate.TabIndex = 14;
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcDesc,
            this.lcStorageFrom,
            this.lcStorageTo,
            this.lcStorageFromQuantity,
            this.lcStorageToQuantity,
            this.lcQuantity,
            this.lcPrice,
            this.lcExpDate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(640, 236);
            this.Root.TextVisible = false;
            //
            // lcDesc
            //
            this.lcDesc.AllowHtmlStringInCaption = true;
            this.lcDesc.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDesc.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDesc.AppearanceItemCaption.Options.UseFont = true;
            this.lcDesc.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDesc.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDesc.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDesc.Control = this.txbDesc;
            this.lcDesc.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcDesc.CustomizationFormText = "中文名稱";
            this.lcDesc.Location = new System.Drawing.Point(0, 108);
            this.lcDesc.Name = "lcDesc";
            this.lcDesc.Size = new System.Drawing.Size(620, 36);
            this.lcDesc.Text = "備註";
            this.lcDesc.TextSize = new System.Drawing.Size(76, 24);
            //
            // lcStorageFrom
            //
            this.lcStorageFrom.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageFrom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcStorageFrom.AppearanceItemCaption.Options.UseFont = true;
            this.lcStorageFrom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcStorageFrom.AppearanceItemCaptionDisabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageFrom.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcStorageFrom.AppearanceItemCaptionDisabled.Options.UseFont = true;
            this.lcStorageFrom.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcStorageFrom.Control = this.cbbStorageFrom;
            this.lcStorageFrom.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcStorageFrom.CustomizationFormText = "備品用途";
            this.lcStorageFrom.Location = new System.Drawing.Point(0, 0);
            this.lcStorageFrom.Name = "lcStorageFrom";
            this.lcStorageFrom.Size = new System.Drawing.Size(440, 36);
            this.lcStorageFrom.Text = "出貨倉庫";
            this.lcStorageFrom.TextSize = new System.Drawing.Size(76, 24);
            //
            // lcStorageTo
            //
            this.lcStorageTo.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageTo.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcStorageTo.AppearanceItemCaption.Options.UseFont = true;
            this.lcStorageTo.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcStorageTo.AppearanceItemCaptionDisabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageTo.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcStorageTo.AppearanceItemCaptionDisabled.Options.UseFont = true;
            this.lcStorageTo.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcStorageTo.Control = this.cbbStorageTo;
            this.lcStorageTo.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcStorageTo.CustomizationFormText = "備品用途";
            this.lcStorageTo.Location = new System.Drawing.Point(0, 36);
            this.lcStorageTo.Name = "lcStorageTo";
            this.lcStorageTo.Size = new System.Drawing.Size(440, 36);
            this.lcStorageTo.Text = "收貨倉庫";
            this.lcStorageTo.TextSize = new System.Drawing.Size(76, 24);
            //
            // lcStorageFromQuantity
            //
            this.lcStorageFromQuantity.AllowHtmlStringInCaption = true;
            this.lcStorageFromQuantity.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageFromQuantity.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcStorageFromQuantity.AppearanceItemCaption.Options.UseFont = true;
            this.lcStorageFromQuantity.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcStorageFromQuantity.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcStorageFromQuantity.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcStorageFromQuantity.Control = this.txbStorageFromQuantity;
            this.lcStorageFromQuantity.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcStorageFromQuantity.CustomizationFormText = "中文名稱";
            this.lcStorageFromQuantity.Location = new System.Drawing.Point(440, 0);
            this.lcStorageFromQuantity.Name = "lcStorageFromQuantity";
            this.lcStorageFromQuantity.Size = new System.Drawing.Size(180, 36);
            this.lcStorageFromQuantity.Text = "數量";
            this.lcStorageFromQuantity.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcStorageFromQuantity.TextSize = new System.Drawing.Size(38, 24);
            this.lcStorageFromQuantity.TextToControlDistance = 5;
            //
            // lcStorageToQuantity
            //
            this.lcStorageToQuantity.AllowHtmlStringInCaption = true;
            this.lcStorageToQuantity.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcStorageToQuantity.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcStorageToQuantity.AppearanceItemCaption.Options.UseFont = true;
            this.lcStorageToQuantity.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcStorageToQuantity.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcStorageToQuantity.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcStorageToQuantity.Control = this.txbStorageToQuantity;
            this.lcStorageToQuantity.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcStorageToQuantity.CustomizationFormText = "中文名稱";
            this.lcStorageToQuantity.Location = new System.Drawing.Point(440, 36);
            this.lcStorageToQuantity.Name = "lcStorageToQuantity";
            this.lcStorageToQuantity.Size = new System.Drawing.Size(180, 36);
            this.lcStorageToQuantity.Text = "數量";
            this.lcStorageToQuantity.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcStorageToQuantity.TextSize = new System.Drawing.Size(38, 24);
            this.lcStorageToQuantity.TextToControlDistance = 5;
            //
            // lcQuantity
            //
            this.lcQuantity.AllowHtmlStringInCaption = true;
            this.lcQuantity.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcQuantity.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcQuantity.AppearanceItemCaption.Options.UseFont = true;
            this.lcQuantity.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcQuantity.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcQuantity.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcQuantity.Control = this.txbQuantity;
            this.lcQuantity.CustomizationFormText = "中文名稱";
            this.lcQuantity.Location = new System.Drawing.Point(0, 72);
            this.lcQuantity.Name = "lcQuantity";
            this.lcQuantity.Size = new System.Drawing.Size(620, 36);
            this.lcQuantity.Text = "事件數量";
            this.lcQuantity.TextSize = new System.Drawing.Size(76, 24);
            //
            // lcPrice
            //
            this.lcPrice.AllowHtmlStringInCaption = true;
            this.lcPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcPrice.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcPrice.AppearanceItemCaption.Options.UseFont = true;
            this.lcPrice.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcPrice.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcPrice.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcPrice.Control = this.txbPrice;
            this.lcPrice.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcPrice.CustomizationFormText = "中文名稱";
            this.lcPrice.Location = new System.Drawing.Point(0, 144);
            this.lcPrice.Name = "lcPrice";
            this.lcPrice.Size = new System.Drawing.Size(620, 36);
            this.lcPrice.Text = "新單價";
            this.lcPrice.TextSize = new System.Drawing.Size(76, 24);
            //
            // lcExpDate
            //
            this.lcExpDate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcExpDate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcExpDate.AppearanceItemCaption.Options.UseFont = true;
            this.lcExpDate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcExpDate.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcExpDate.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcExpDate.Control = this.txbExpDate;
            this.lcExpDate.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcExpDate.CustomizationFormText = "有效期至";
            this.lcExpDate.Location = new System.Drawing.Point(0, 180);
            this.lcExpDate.Name = "lcExpDate";
            this.lcExpDate.Size = new System.Drawing.Size(620, 36);
            this.lcExpDate.Text = "有效期至";
            this.lcExpDate.TextSize = new System.Drawing.Size(76, 24);
            //
            // SparePartTransactionForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 285);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Icon = global::Winform4System.Properties.Resources.AppIcon;
            this.Name = "SparePartTransactionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "進出庫資料";
            this.Load += new System.EventHandler(this.SparePartTransactionForm_Load);

            cbbRecoveryOption = new ImageComboBoxEdit();
            cbbRecoveryOption.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            cbbRecoveryOption.Properties.Appearance.ForeColor = Color.Black;
            cbbRecoveryOption.Properties.Appearance.Options.UseFont = true;
            cbbRecoveryOption.Properties.Appearance.Options.UseForeColor = true;
            cbbRecoveryOption.Properties.AppearanceDropDown.Font = new Font("Microsoft JhengHei UI", 14.25F);
            cbbRecoveryOption.Properties.AppearanceDropDown.Options.UseFont = true;
            cbbRecoveryOption.Properties.Buttons.Clear();
            cbbRecoveryOption.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("沒有舊品拆出", RecoveryOptionNoneValue, -1));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("拆出後送報廢", RecoveryOptionScrapValue, -1));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("拆出後回收入庫", RecoveryOptionRestockValue, -1));
            cbbRecoveryOption.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cbbRecoveryOption.EditValue = RecoveryOptionNoneValue;
            cbbRecoveryOption.Enabled = false;
            cbbRecoveryOption.EditValueChanged += cbbRecoveryOption_EditValueChanged;

            sleAssignedUser = new SearchLookUpEdit();
            sleAssignedUser.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            sleAssignedUser.Properties.Appearance.ForeColor = Color.Black;
            sleAssignedUser.Properties.Appearance.Options.UseFont = true;
            sleAssignedUser.Properties.Appearance.Options.UseForeColor = true;
            sleAssignedUser.Properties.AppearanceDropDown.Font = new Font("Microsoft JhengHei UI", 14.25F);
            sleAssignedUser.Properties.AppearanceDropDown.Options.UseFont = true;
            sleAssignedUser.Properties.NullText = string.Empty;
            gvAssignedUserLookup = new GridView();
            colAssignedUserId = new DevExpress.XtraGrid.Columns.GridColumn { Name = "colAssignedUserId", FieldName = "Id", Caption = "工號", Visible = true, VisibleIndex = 0, Width = 120 };
            colAssignedUserName = new DevExpress.XtraGrid.Columns.GridColumn { Name = "colAssignedUserName", FieldName = "DisplayName", Caption = "姓名", Visible = true, VisibleIndex = 1, Width = 180 };
            colAssignedUserDepartment = new DevExpress.XtraGrid.Columns.GridColumn { Name = "colAssignedUserDepartment", FieldName = "IdDepartment", Caption = "單位", Visible = true, VisibleIndex = 2, Width = 140 };
            gvAssignedUserLookup.Columns.AddRange(new[] { colAssignedUserId, colAssignedUserName, colAssignedUserDepartment });
            gvAssignedUserLookup.Name = "gvAssignedUserLookup";
            gvAssignedUserLookup.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F);
            gvAssignedUserLookup.Appearance.HeaderPanel.Options.UseFont = true;
            gvAssignedUserLookup.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gvAssignedUserLookup.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gvAssignedUserLookup.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F);
            gvAssignedUserLookup.Appearance.Row.Options.UseFont = true;
            gvAssignedUserLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gvAssignedUserLookup.OptionsBehavior.Editable = false;
            gvAssignedUserLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvAssignedUserLookup.OptionsView.ShowAutoFilterRow = true;
            gvAssignedUserLookup.OptionsView.ShowGroupPanel = false;
            gvAssignedUserLookup.OptionsView.ColumnAutoWidth = false;
            sleAssignedUser.Properties.PopupView = gvAssignedUserLookup;
            sleAssignedUser.Properties.DisplayMember = "DisplayName";
            sleAssignedUser.Properties.ValueMember = "Id";

            dePlannedDisposeDate = new DateEdit();
            dePlannedDisposeDate.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            dePlannedDisposeDate.Properties.Appearance.ForeColor = Color.Black;
            dePlannedDisposeDate.Properties.Appearance.Options.UseFont = true;
            dePlannedDisposeDate.Properties.Appearance.Options.UseForeColor = true;
            dePlannedDisposeDate.Properties.Buttons.Clear();
            dePlannedDisposeDate.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            dePlannedDisposeDate.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
            dePlannedDisposeDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
            dePlannedDisposeDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            dePlannedDisposeDate.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
            dePlannedDisposeDate.Properties.UseMaskAsDisplayFormat = true;
            dePlannedDisposeDate.Properties.CalendarTimeProperties.Buttons.Clear();
            dePlannedDisposeDate.Properties.CalendarTimeProperties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));

            cbbRestockStorage = new LookUpEdit();
            cbbRestockStorage.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            cbbRestockStorage.Properties.Appearance.ForeColor = Color.Black;
            cbbRestockStorage.Properties.Appearance.Options.UseFont = true;
            cbbRestockStorage.Properties.Appearance.Options.UseForeColor = true;
            cbbRestockStorage.Properties.AppearanceDropDown.Font = new Font("Microsoft JhengHei UI", 14.25F);
            cbbRestockStorage.Properties.AppearanceDropDown.Options.UseFont = true;
            cbbRestockStorage.Properties.Buttons.Clear();
            cbbRestockStorage.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            cbbRestockStorage.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[]
            {
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "倉庫")
            });
            cbbRestockStorage.Properties.DisplayMember = "DisplayName";
            cbbRestockStorage.Properties.ValueMember = "Id";
            cbbRestockStorage.Properties.NullText = string.Empty;
            cbbRestockStorage.Properties.ShowHeader = false;
            cbbRestockStorage.Properties.DropDownRows = 5;

            memoRecoveryNote = new MemoEdit();
            memoRecoveryNote.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            memoRecoveryNote.Properties.Appearance.ForeColor = Color.Black;
            memoRecoveryNote.Properties.Appearance.Options.UseFont = true;
            memoRecoveryNote.Properties.Appearance.Options.UseForeColor = true;

            btnViewGuideDocs = new SimpleButton();
            btnViewGuideDocs.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            btnViewGuideDocs.Appearance.Options.UseFont = true;
            btnViewGuideDocs.Text = "查看報廢指引";
            btnViewGuideDocs.Click += btnViewGuideDocs_Click;

            layoutControl1.Controls.Add(cbbRecoveryOption);
            layoutControl1.Controls.Add(sleAssignedUser);
            layoutControl1.Controls.Add(dePlannedDisposeDate);
            layoutControl1.Controls.Add(cbbRestockStorage);
            layoutControl1.Controls.Add(memoRecoveryNote);
            layoutControl1.Controls.Add(btnViewGuideDocs);

            lcRecoveryOption = new LayoutControlItem();
            lcRecoveryOption.Name = "lcRecoveryOption";
            lcRecoveryOption.Control = cbbRecoveryOption;
            lcRecoveryOption.Text = "舊品處理";
            lcRecoveryOption.TextVisible = true;
            lcRecoveryOption.AllowHtmlStringInCaption = true;
            lcRecoveryOption.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcRecoveryOption.AppearanceItemCaption.ForeColor = Color.Black;
            lcRecoveryOption.AppearanceItemCaption.Options.UseFont = true;
            lcRecoveryOption.AppearanceItemCaption.Options.UseForeColor = true;
            lcRecoveryOption.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcRecoveryOption.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcRecoveryOption.TextSize = new Size(76, 24);
            lcRecoveryOption.Location = new Point(0, 216);
            lcRecoveryOption.Size = new Size(620, 36);
            lcRecoveryOption.SizeConstraintsType = SizeConstraintsType.Custom;
            lcRecoveryOption.MinSize = new Size(200, 36);
            lcRecoveryOption.MaxSize = new Size(0, 36);
            lcRecoveryOption.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcAssignedUser = new LayoutControlItem();
            lcAssignedUser.Name = "lcAssignedUser";
            lcAssignedUser.Control = sleAssignedUser;
            lcAssignedUser.Text = "報廢經辦";
            lcAssignedUser.TextVisible = true;
            lcAssignedUser.AllowHtmlStringInCaption = true;
            lcAssignedUser.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcAssignedUser.AppearanceItemCaption.ForeColor = Color.Black;
            lcAssignedUser.AppearanceItemCaption.Options.UseFont = true;
            lcAssignedUser.AppearanceItemCaption.Options.UseForeColor = true;
            lcAssignedUser.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcAssignedUser.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcAssignedUser.TextSize = new Size(76, 24);
            lcAssignedUser.Location = new Point(0, 252);
            lcAssignedUser.Size = new Size(620, 36);
            lcAssignedUser.SizeConstraintsType = SizeConstraintsType.Custom;
            lcAssignedUser.MinSize = new Size(200, 36);
            lcAssignedUser.MaxSize = new Size(0, 36);
            lcAssignedUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcPlannedDisposeDate = new LayoutControlItem();
            lcPlannedDisposeDate.Name = "lcPlannedDisposeDate";
            lcPlannedDisposeDate.Control = dePlannedDisposeDate;
            lcPlannedDisposeDate.Text = "預計日期";
            lcPlannedDisposeDate.TextVisible = true;
            lcPlannedDisposeDate.AllowHtmlStringInCaption = true;
            lcPlannedDisposeDate.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcPlannedDisposeDate.AppearanceItemCaption.ForeColor = Color.Black;
            lcPlannedDisposeDate.AppearanceItemCaption.Options.UseFont = true;
            lcPlannedDisposeDate.AppearanceItemCaption.Options.UseForeColor = true;
            lcPlannedDisposeDate.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcPlannedDisposeDate.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcPlannedDisposeDate.TextSize = new Size(76, 24);
            lcPlannedDisposeDate.Location = new Point(0, 288);
            lcPlannedDisposeDate.Size = new Size(620, 36);
            lcPlannedDisposeDate.SizeConstraintsType = SizeConstraintsType.Custom;
            lcPlannedDisposeDate.MinSize = new Size(200, 36);
            lcPlannedDisposeDate.MaxSize = new Size(0, 36);
            lcPlannedDisposeDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRestockStorage = new LayoutControlItem();
            lcRestockStorage.Name = "lcRestockStorage";
            lcRestockStorage.Control = cbbRestockStorage;
            lcRestockStorage.Text = "回收倉庫";
            lcRestockStorage.TextVisible = true;
            lcRestockStorage.AllowHtmlStringInCaption = true;
            lcRestockStorage.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcRestockStorage.AppearanceItemCaption.ForeColor = Color.Black;
            lcRestockStorage.AppearanceItemCaption.Options.UseFont = true;
            lcRestockStorage.AppearanceItemCaption.Options.UseForeColor = true;
            lcRestockStorage.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcRestockStorage.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcRestockStorage.TextSize = new Size(76, 24);
            lcRestockStorage.Location = new Point(0, 324);
            lcRestockStorage.Size = new Size(620, 36);
            lcRestockStorage.SizeConstraintsType = SizeConstraintsType.Custom;
            lcRestockStorage.MinSize = new Size(200, 36);
            lcRestockStorage.MaxSize = new Size(0, 36);
            lcRestockStorage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRecoveryNote = new LayoutControlItem();
            lcRecoveryNote.Name = "lcRecoveryNote";
            lcRecoveryNote.Control = memoRecoveryNote;
            lcRecoveryNote.Text = "回收備註";
            lcRecoveryNote.TextVisible = true;
            lcRecoveryNote.AllowHtmlStringInCaption = true;
            lcRecoveryNote.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcRecoveryNote.AppearanceItemCaption.ForeColor = Color.Black;
            lcRecoveryNote.AppearanceItemCaption.Options.UseFont = true;
            lcRecoveryNote.AppearanceItemCaption.Options.UseForeColor = true;
            lcRecoveryNote.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcRecoveryNote.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcRecoveryNote.TextSize = new Size(76, 24);
            lcRecoveryNote.Location = new Point(0, 360);
            lcRecoveryNote.Size = new Size(620, 84);
            lcRecoveryNote.SizeConstraintsType = SizeConstraintsType.Custom;
            lcRecoveryNote.MinSize = new Size(200, 84);
            lcRecoveryNote.MaxSize = new Size(0, 84);
            lcRecoveryNote.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcGuideDocs = new LayoutControlItem();
            lcGuideDocs.Name = "lcGuideDocs";
            lcGuideDocs.Control = btnViewGuideDocs;
            lcGuideDocs.Text = "";
            lcGuideDocs.TextVisible = false;
            lcGuideDocs.AllowHtmlStringInCaption = true;
            lcGuideDocs.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            lcGuideDocs.AppearanceItemCaption.ForeColor = Color.Black;
            lcGuideDocs.AppearanceItemCaption.Options.UseFont = true;
            lcGuideDocs.AppearanceItemCaption.Options.UseForeColor = true;
            lcGuideDocs.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            lcGuideDocs.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            lcGuideDocs.TextSize = new Size(76, 24);
            lcGuideDocs.Location = new Point(0, 444);
            lcGuideDocs.Size = new Size(620, 36);
            lcGuideDocs.SizeConstraintsType = SizeConstraintsType.Custom;
            lcGuideDocs.MinSize = new Size(200, 36);
            lcGuideDocs.MaxSize = new Size(0, 36);
            lcGuideDocs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lcRecoveryOption, lcAssignedUser, lcPlannedDisposeDate, lcRestockStorage, lcRecoveryNote, lcGuideDocs });

            cbbRecoveryOption.Name = "cbbRecoveryOption"; cbbRecoveryOption.TabIndex = 8;

            sleAssignedUser.Name = "sleAssignedUser"; sleAssignedUser.TabIndex = 9;

            dePlannedDisposeDate.Name = "dePlannedDisposeDate"; dePlannedDisposeDate.TabIndex = 10;

            cbbRestockStorage.Name = "cbbRestockStorage"; cbbRestockStorage.TabIndex = 11;

            memoRecoveryNote.Name = "memoRecoveryNote"; memoRecoveryNote.TabIndex = 12;

            btnViewGuideDocs.Name = "btnViewGuideDocs"; btnViewGuideDocs.TabIndex = 13;
            btnViewGuideDocs.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Attachment;
            lcStorageFromQuantity.SizeConstraintsType = SizeConstraintsType.Custom;
            lcStorageFromQuantity.MinSize = new Size(180, 36); lcStorageFromQuantity.MaxSize = new Size(180, 0);
            lcStorageToQuantity.SizeConstraintsType = SizeConstraintsType.Custom;
            lcStorageToQuantity.MinSize = new Size(180, 36); lcStorageToQuantity.MaxSize = new Size(180, 0);

            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbQuantity.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStorageFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbStorageFromQuantity.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStorageTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbStorageToQuantity.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExpDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExpDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageFromQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStorageToQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExpDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnEdit;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbQuantity;
        private DevExpress.XtraEditors.TextEdit txbDesc;
        private DevExpress.XtraEditors.LookUpEdit cbbStorageFrom;
        private DevExpress.XtraEditors.TextEdit txbStorageFromQuantity;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcStorageFrom;
        private DevExpress.XtraLayout.LayoutControlItem lcQuantity;
        private DevExpress.XtraLayout.LayoutControlItem lcDesc;
        private DevExpress.XtraLayout.LayoutControlItem lcStorageFromQuantity;
        private DevExpress.XtraEditors.LookUpEdit cbbStorageTo;
        private DevExpress.XtraLayout.LayoutControlItem lcStorageTo;
        private DevExpress.XtraEditors.TextEdit txbStorageToQuantity;
        private DevExpress.XtraLayout.LayoutControlItem lcStorageToQuantity;
        private DevExpress.XtraEditors.TextEdit txbPrice;
        private DevExpress.XtraLayout.LayoutControlItem lcPrice;
        private DevExpress.XtraEditors.DateEdit txbExpDate;
        private DevExpress.XtraLayout.LayoutControlItem lcExpDate;
    }
}
