using DataAccessLayer;
using DevExpress.XtraEditors;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Winform4System.Core.Models;

namespace KnowledgeSystem.Helpers
{
    public enum EventFormInfo
    {
        Create,
        View,
        Update,
        Delete,
        ViewOnly
    }

    public static class TPConfigs
    {
        public static readonly string TempFolderData = Path.Combine(Path.GetTempPath(), "Winform4System", "Temp");
        public static readonly Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public const string FilterFile = "檔案|*.pdf;*.xlsx;*.xls;*.docx;*.doc;*.ppt;*.pptx;*.jpg;*.jpeg;*.png";

        public static string SoftNameTW => Winform4System.ApplicationMetadata.DisplayName;
        public static string idDept2word { get; private set; }
        public static string Folder309 { get; private set; }
        public static string FolderReportFormat { get; private set; }
        public static dm_User LoginUser { get; private set; }

        public static void Initialize(UserSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            string departmentCode = session.DepartmentCode ?? string.Empty;
            idDept2word = departmentCode.Length >= 2 ? departmentCode.Substring(0, 2) : departmentCode;
            LoginUser = new dm_User
            {
                Id = session.UserId,
                DisplayName = session.DisplayNameTW,
                DisplayNameVN = session.DisplayNameVN,
                IdDepartment = departmentCode,
                Status = 0
            };

            string configuredPath = ConfigurationManager.AppSettings["SparePartDataPath"];
            string dataRoot = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SoftNameTW, "Data")
                : configuredPath;
            Folder309 = Path.Combine(dataRoot, "309");
            FolderReportFormat = Path.Combine(dataRoot, "00", "ReportFormat");
        }

        public static string DocumentPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SoftNameTW);
            Directory.CreateDirectory(path);
            return path;
        }
    }

    public static class MsgTP
    {
        public static void MsgErrorDB()
        {
            XtraMessageBox.Show("資料處理失敗，請確認輸入內容或聯絡系統管理員。",
                TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void MsgConfirmDel()
        {
            XtraMessageBox.Show("請再次確認要刪除的資料，然後按下確認。",
                TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void MsgError(string message)
        {
            XtraMessageBox.Show(message, TPConfigs.SoftNameTW,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
