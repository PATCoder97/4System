using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services.SpareParts;
using Winform4System.Core.Security;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public static class SparePartHelper
    {
        private const string MaterialPhotoFolder = "MaterialPhotos";

        public static bool CanManageAllDepartments()
        {
            return CurrentAuthorization.HasPermission("ASSET.SPARE_PART.ADMIN")
                || CurrentAuthorization.HasPermission("ASSET.SPARE_PART.APPROVE");
        }

        public static List<SparePartDepartment> GetAccessibleDepartments()
        {
            var departments = SparePartDepartmentService.Instance.GetList();
            if (CanManageAllDepartments())
                return departments.OrderBy(department => department.Id).ToList();

            string departmentCode = SparePartConfiguration.LoginUser?.IdDepartment ?? string.Empty;
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
            Directory.CreateDirectory(SparePartConfiguration.SparePartFolder);
            return SparePartConfiguration.SparePartFolder;
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
            string encryptionName = StoredFileWriter.CopyToStorage(
                sourceFilePath,
                EnsureMaterialPhotoFolder(materialId));
            return (encryptionName, actualName);
        }

        public static string GetMaterialPhotoPath(SparePartMaterialPhoto photo)
        {
            return Path.Combine(EnsureMaterialPhotoFolder(photo.MaterialId), photo.EncryptionName);
        }

        public static string CopyToTemp(string sourcePath, string actualName)
        {
            Directory.CreateDirectory(SparePartConfiguration.TempFolderData);
            string extension = Path.GetExtension(actualName) ?? string.Empty;
            string tempFile = Path.Combine(
                SparePartConfiguration.TempFolderData,
                $"{Guid.NewGuid():N}{extension}");
            File.Copy(sourcePath, tempFile, true);
            return tempFile;
        }

        public static void OpenPhotoFile(string physicalPath, string actualName)
        {
            if (!File.Exists(physicalPath))
            {
                XtraMessageBox.Show("找不到對應的照片檔案。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo(CopyToTemp(physicalPath, actualName)) { UseShellExecute = true });
        }
    }
}
