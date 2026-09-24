using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;
using System;
using System.Windows.Forms;
using Winform4System.Core.Models;
using Winform4System.Core.Security;

namespace Winform4System.Forms.SpareParts
{
    public sealed class SparePartForm : XtraForm
    {
        public SparePartForm(UserSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            TPConfigs.Initialize(session);
            Text = "309 備品備件管理";
            IconOptions.Icon = Properties.Resources.AppIcon;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new System.Drawing.Size(1100, 700);

            var tabs = new XtraTabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            AddPage(tabs, "配件資料", new uc309_SparePartMain());
            AddPage(tabs, "設備管理", new uc309_MachineMgmt());
            AddPage(tabs, "進出庫管理", new uc309_InOutMgmt());
            AddPage(tabs, "盤點批次", new uc309_InspectionBatch());
            AddPage(tabs, "複盤作業", new uc309_RecheckBatch());
            AddPage(tabs, "成本計算", new uc309_CostCalculation());
            AddPage(tabs, "回收管理", new uc309_RecoveryMgmt());
            AddPage(tabs, "我的回收作業", new uc309_RecoveryTask());
        }

        private static void AddPage(XtraTabControl tabs, string caption, Control content)
        {
            var page = new XtraTabPage { Text = caption };
            content.Dock = DockStyle.Fill;
            page.Controls.Add(content);
            tabs.TabPages.Add(page);
        }
    }
}
