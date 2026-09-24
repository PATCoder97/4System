using DataAccessLayer;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BusinessLayer;
using Winform4System.Core.Security;

namespace KnowledgeSystem.Helpers
{
    public static class Material309Helper
    {
        private const string MaterialPhotoFolder = "MaterialPhotos";

        public static bool CanManageAllDepartments()
        {
            return CurrentAuthorization.HasPermission("ASSET.SPARE_PART.ADMIN")
                || CurrentAuthorization.HasPermission("ASSET.SPARE_PART.APPROVE");
        }

        public static List<dm_Departments> GetAccessibleDepartments()
        {
            var departments = dm_DeptBUS.Instance.GetList();
            if (CanManageAllDepartments())
                return departments.OrderBy(department => department.Id).ToList();

            string departmentCode = TPConfigs.LoginUser?.IdDepartment ?? string.Empty;
            string departmentPrefix = departmentCode.Length >= 2
                ? departmentCode.Substring(0, 2)
                : departmentCode;

            return departments
                .Where(department => !string.IsNullOrWhiteSpace(department.Id)
                    && (department.Id == departmentCode
                        || (!string.IsNullOrWhiteSpace(departmentPrefix)
                            && department.Id.StartsWith(departmentPrefix, StringComparison.OrdinalIgnoreCase))))
                .OrderBy(department => department.Id)
                .ToList();
        }

        public static string EnsureBaseFolder()
        {
            Directory.CreateDirectory(TPConfigs.Folder309);
            return TPConfigs.Folder309;
        }

        public static string EnsureMaterialPhotoFolder(int materialId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), MaterialPhotoFolder, materialId.ToString());
            Directory.CreateDirectory(folder);
            return folder;
        }

        public static (string encryptionName, string actualName) SaveMaterialPhoto(int materialId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            File.Copy(sourceFilePath, Path.Combine(EnsureMaterialPhotoFolder(materialId), encryptionName), true);
            return (encryptionName, actualName);
        }

        public static string GetMaterialPhotoPath(dt309_MaterialPhoto photo)
        {
            return Path.Combine(EnsureMaterialPhotoFolder(photo.MaterialId), photo.EncryptionName);
        }

        public static string CopyToTemp(string sourcePath, string actualName)
        {
            Directory.CreateDirectory(TPConfigs.TempFolderData);
            string tempFile = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyyyMMddHHmmssfff}-{actualName}");
            File.Copy(sourcePath, tempFile, true);
            return tempFile;
        }

        public static void OpenPhotoFile(string physicalPath, string actualName)
        {
            if (!File.Exists(physicalPath))
            {
                XtraMessageBox.Show("找不到對應的照片檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo(CopyToTemp(physicalPath, actualName)) { UseShellExecute = true });
        }
    }
}
