using DevExpress.Utils;
using DevExpress.XtraBars.FluentDesignSystem;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Winform4System.Core.Models;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public sealed class SparePartForm : FluentDesignForm
    {
        private readonly AccordionControl _navigation;
        private readonly FluentDesignFormContainer _container;
        private readonly FluentDesignFormControl _titleBar;
        private readonly XtraTabControl _tabs;
        private readonly Dictionary<string, XtraTabPage> _openPages =
            new Dictionary<string, XtraTabPage>(StringComparer.OrdinalIgnoreCase);

        public SparePartForm(UserSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            SparePartConfiguration.Initialize(session);
            Text = "備品備件管理";
            IconOptions.Icon = Properties.Resources.AppIcon;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);

            _titleBar = new FluentDesignFormControl
            {
                Dock = DockStyle.Top,
                FluentDesignForm = this,
                Name = "sparePartTitleBar"
            };
            _navigation = new AccordionControl
            {
                AllowItemSelection = true,
                Dock = DockStyle.Left,
                Name = "sparePartNavigation",
                ScrollBarMode = ScrollBarMode.Touch,
                ShowFilterControl = ShowFilterControl.Always,
                ViewType = AccordionControlViewType.HamburgerMenu,
                Width = 235
            };
            _container = new FluentDesignFormContainer
            {
                Dock = DockStyle.Fill,
                Name = "sparePartContainer"
            };
            _tabs = new XtraTabControl
            {
                Dock = DockStyle.Fill,
                Name = "sparePartTabs",
                ClosePageButtonShowMode = ClosePageButtonShowMode.InAllTabPageHeaders,
                ShowTabHeader = DefaultBoolean.True
            };
            _tabs.AppearancePage.Header.Font = new Font("Microsoft JhengHei UI", 12F);
            _tabs.AppearancePage.Header.Options.UseFont = true;
            _tabs.AppearancePage.HeaderActive.Font = new Font("Microsoft JhengHei UI", 12F);
            _tabs.AppearancePage.HeaderActive.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            _tabs.AppearancePage.HeaderActive.Options.UseFont = true;
            _tabs.AppearancePage.HeaderActive.Options.UseForeColor = true;
            _tabs.CloseButtonClick += Tabs_CloseButtonClick;
            _container.Controls.Add(_tabs);

            Controls.Add(_container);
            Controls.Add(_navigation);
            Controls.Add(_titleBar);
            ControlContainer = _container;
            FluentDesignFormControl = _titleBar;
            NavigationControl = _navigation;

            BuildNavigation();
            Shown += (sender, args) => OpenTab("materials", "備品資料", () => new SparePartMaterialView());
        }

        private void BuildNavigation()
        {
            var group = new AccordionControlElement
            {
                Text = "備品備件功能",
                Style = ElementStyle.Group,
                Expanded = true
            };
            group.Appearance.Default.Font = new Font("Microsoft JhengHei UI", 12F);

            AddNavigationItem(group, "materials", "備品資料", SvgImageCatalog.View, () => new SparePartMaterialView());
            AddNavigationItem(group, "machines", "設備管理", SvgImageCatalog.Gears, () => new SparePartMachineView());
            AddNavigationItem(group, "transactions", "進出庫管理", SvgImageCatalog.Transfer, () => new SparePartTransactionView());
            AddNavigationItem(group, "inspection", "盤點批次", SvgImageCatalog.CheckedRadio, () => new SparePartInspectionView());
            AddNavigationItem(group, "recheck", "複盤作業", SvgImageCatalog.Reload, () => new SparePartRecheckView());
            AddNavigationItem(group, "cost", "成本計算", SvgImageCatalog.Money, () => new SparePartCostView());
            AddNavigationItem(group, "recovery", "回收管理", SvgImageCatalog.Schedule, () => new SparePartRecoveryView());
            AddNavigationItem(group, "my-recovery", "我的回收作業", SvgImageCatalog.PersonnelChanges, () => new SparePartRecoveryTaskView());
            _navigation.Elements.Add(group);
        }

        private static Font NavigationFont => new Font("Microsoft JhengHei UI", 12F);

        private void AddNavigationItem(
            AccordionControlElement parent,
            string key,
            string caption,
            DevExpress.Utils.Svg.SvgImage icon,
            Func<Control> contentFactory)
        {
            var item = new AccordionControlElement
            {
                Name = "nav_" + key,
                Text = caption,
                Hint = caption,
                Style = ElementStyle.Item,
                Tag = new SparePartNavigationItem(key, caption, contentFactory)
            };
            item.Appearance.Default.Font = NavigationFont;
            item.Appearance.Normal.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Hyperlink;
            item.Appearance.Hovered.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            item.Appearance.Pressed.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            item.ImageOptions.SvgImage = icon;
            item.Click += NavigationItem_Click;
            parent.Elements.Add(item);
        }

        private void NavigationItem_Click(object sender, EventArgs e)
        {
            var element = sender as AccordionControlElement;
            var navigationItem = element?.Tag as SparePartNavigationItem;
            if (navigationItem != null)
                OpenTab(navigationItem.Key, navigationItem.Caption, navigationItem.ContentFactory);
        }

        private void OpenTab(string key, string caption, Func<Control> contentFactory)
        {
            if (_openPages.TryGetValue(key, out XtraTabPage existingPage))
            {
                _tabs.SelectedTabPage = existingPage;
                return;
            }

            using (SplashScreenManager.ShowOverlayForm(this))
            {
                Control content = contentFactory();
                content.Dock = DockStyle.Fill;
                var page = new XtraTabPage { Name = "tab_" + key, Text = caption, Tag = key };
                page.Controls.Add(content);
                _tabs.TabPages.Add(page);
                _openPages.Add(key, page);
                _tabs.SelectedTabPage = page;
            }
        }

        private void Tabs_CloseButtonClick(object sender, EventArgs e)
        {
            var pageProperty = e?.GetType().GetProperty("Page");
            var page = pageProperty?.GetValue(e, null) as XtraTabPage ?? _tabs.SelectedTabPage;
            if (page == null)
                return;

            string key = page.Tag as string;
            if (!string.IsNullOrWhiteSpace(key))
                _openPages.Remove(key);
            _tabs.TabPages.Remove(page);
            page.Dispose();
        }

        private sealed class SparePartNavigationItem
        {
            public SparePartNavigationItem(string key, string caption, Func<Control> contentFactory)
            {
                Key = key;
                Caption = caption;
                ContentFactory = contentFactory;
            }

            public string Key { get; }
            public string Caption { get; }
            public Func<Control> ContentFactory { get; }
        }
    }
}
