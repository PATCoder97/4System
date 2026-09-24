using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Models;
using Winform4System.Logging;
using Winform4System.Forms.SpareParts;
using Winform4System.Core.Security;

namespace Winform4System.Forms.Main
{
    public partial class MainForm : XtraForm
    {
        private static readonly Color NotStartedColor = Color.FromArgb(128, 57, 123);
        private static readonly Color InProgressColor = Color.FromArgb(183, 71, 42);
        private static readonly Color CompletedColor = Color.FromArgb(16, 110, 190);

        private readonly IMainMenuService _menuService;
        private readonly IAppLogger _logger;
        private readonly UserSession _session;
        private readonly Dictionary<TileItem, MenuItemDefinition> _menuByTile = new Dictionary<TileItem, MenuItemDefinition>();

        public bool LogoutRequested { get; private set; }

        public MainForm(IMainMenuService menuService, IAppLogger logger, UserSession session)
        {
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            lblWelcome.Text = $"您好，{_session.DisplayName}，";
            lblSession.Text = $"{_session.UserId}  •  {_session.Department}  •  {_session.Role}";
            LayoutUserLinks();

            IReadOnlyList<MenuItemDefinition> menuItems = _menuService.GetMenuItems();
            foreach (IGrouping<string, MenuItemDefinition> groupData in menuItems.GroupBy(item => item.Group))
            {
                var group = new TileGroup { Text = groupData.Key };
                foreach (MenuItemDefinition definition in groupData)
                    group.Items.Add(CreateMenuTile(definition));

                tileMain.Groups.Add(group);
            }

            _logger.Info(nameof(MainForm), "Main dashboard initialized.");
        }

        private TileItem CreateMenuTile(MenuItemDefinition definition)
        {
            var titleElement = new TileItemElement
            {
                Text = definition.Title,
                TextAlignment = TileItemContentAlignment.TopLeft
            };
            titleElement.Appearance.Normal.Font = new Font("DFKai-SB", 26F, FontStyle.Regular);
            titleElement.Appearance.Normal.Options.UseFont = true;

            var descriptionElement = new TileItemElement
            {
                Text = definition.Description,
                TextAlignment = TileItemContentAlignment.BottomLeft,
                TextLocation = new Point(0, -2)
            };
            descriptionElement.Appearance.Normal.Font = new Font("DFKai-SB", 12F);
            descriptionElement.Appearance.Normal.ForeColor = Color.FromArgb(235, 245, 250);
            descriptionElement.Appearance.Normal.Options.UseFont = true;
            descriptionElement.Appearance.Normal.Options.UseForeColor = true;

            var tile = new TileItem
            {
                Name = "tile" + definition.Code,
                ItemSize = definition.IsWide ? TileItemSize.Wide : TileItemSize.Medium
            };
            tile.AppearanceItem.Normal.BackColor = GetDevelopmentStatusColor(definition.DevelopmentStatus);
            tile.AppearanceItem.Normal.BorderColor = Color.FromArgb(45, 255, 255, 255);
            tile.AppearanceItem.Normal.Options.UseBackColor = true;
            tile.AppearanceItem.Normal.Options.UseBorderColor = true;
            tile.Elements.Add(titleElement);
            tile.Elements.Add(descriptionElement);
            tile.ItemClick += MenuTile_ItemClick;

            _menuByTile[tile] = definition;
            return tile;
        }

        private static Color GetDevelopmentStatusColor(MenuDevelopmentStatus status)
        {
            switch (status)
            {
                case MenuDevelopmentStatus.Completed:
                    return CompletedColor;
                case MenuDevelopmentStatus.InProgress:
                    return InProgressColor;
                default:
                    return NotStartedColor;
            }
        }

        private void MenuTile_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!_menuByTile.TryGetValue(e.Item, out MenuItemDefinition definition))
                return;

            _logger.Info(nameof(MainForm), $"Menu selected: {definition.Code} - {definition.Title}");
            if (string.Equals(definition.Code, "ASSET.SPARE_PART", StringComparison.OrdinalIgnoreCase))
            {
                if (!CurrentAuthorization.HasPermission("ASSET.SPARE_PART.VIEW"))
                {
                    XtraMessageBox.Show(
                        "您沒有檢視此功能的權限。",
                        ApplicationMetadata.DisplayName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                using (var form = new SparePartForm(_session))
                    form.ShowDialog(this);
                return;
            }

            XtraMessageBox.Show(
                $"「{definition.Title}」功能已完成介面配置。\n\n業務功能將於下一階段連接。",
                ApplicationMetadata.DisplayName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show(
                $"{_session.DisplayName}\n使用者代碼：{_session.UserId}\n部門：{_session.Department}\n狀態：{_session.Role}",
                ApplicationMetadata.DisplayName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void headerPanel_SizeChanged(object sender, EventArgs e) => LayoutUserLinks();

        private void LayoutUserLinks()
        {
            const int rightMargin = 28;
            const int top = 8;
            const int linkGap = 8;
            const int appNameGap = 24;
            int right = headerPanel.ClientSize.Width - rightMargin;

            lblLogout.Location = new Point(right - lblLogout.Width, top);

            int welcomeLeft = lblAppName.Right + appNameGap;
            int welcomeRight = lblLogout.Left - linkGap;
            lblWelcome.Location = new Point(welcomeLeft, top);
            lblWelcome.Size = new Size(Math.Max(0, welcomeRight - welcomeLeft), 22);
        }

        private void lblLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = XtraMessageBox.Show(
                "確定要登出系統嗎？",
                ApplicationMetadata.DisplayName,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            _logger.Info(nameof(MainForm), $"User logout requested: {_session.UserId}");
            LogoutRequested = true;
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.Info(
                nameof(MainForm),
                LogoutRequested ? "User session closing for logout." : "Application closing.");
        }
    }
}
