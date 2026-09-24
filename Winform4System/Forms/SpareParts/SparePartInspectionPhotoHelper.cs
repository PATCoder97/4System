using Winform4System.DataAccess.Entities.SpareParts;
using System.IO;

namespace Winform4System.Forms.SpareParts
{
    public static class SparePartInspectionPhotoHelper
    {
        private const string InspectionCheckPhotoFolder = "InspectionCheckPhotos";

        public static string GetThread(int batchMaterialId)
        {
            return $"SPAREPART_CHECK_{batchMaterialId}";
        }

        public static string EnsureInspectionPhotoFolder(int batchMaterialId)
        {
            string folder = Path.Combine(SparePartHelper.EnsureBaseFolder(), InspectionCheckPhotoFolder, batchMaterialId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName) SavePhoto(int batchMaterialId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = SparePartEncryptionHelper.EncryptionFileName(sourceFilePath);
            string folder = EnsureInspectionPhotoFolder(batchMaterialId);
            File.Copy(sourceFilePath, Path.Combine(folder, encryptionName), true);
            return (encryptionName, actualName);
        }

        public static string GetPhotoPath(int batchMaterialId, SparePartInspectionAttachment attachment)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.EncryptionName))
            {
                return string.Empty;
            }

            return Path.Combine(EnsureInspectionPhotoFolder(batchMaterialId), attachment.EncryptionName);
        }

        public static void DeletePhotoFile(int batchMaterialId, SparePartInspectionAttachment attachment)
        {
            string path = GetPhotoPath(batchMaterialId, attachment);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void OpenPhotoFile(int batchMaterialId, SparePartInspectionAttachment attachment)
        {
            if (attachment == null)
            {
                return;
            }

            SparePartHelper.OpenPhotoFile(GetPhotoPath(batchMaterialId, attachment), attachment.ActualName);
        }
    }
}

