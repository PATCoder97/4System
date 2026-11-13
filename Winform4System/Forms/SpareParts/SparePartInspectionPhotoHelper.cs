using Winform4System.DataAccess.Entities.SpareParts;
using System.IO;

using Winform4System.Helpers;

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
            string folder = EnsureInspectionPhotoFolder(batchMaterialId);
            string encryptionName = StoredFileWriter.CopyToStorage(sourceFilePath, folder);
            return (encryptionName, actualName);
        }

        public static string GetPhotoPath(int batchMaterialId, SparePartInspectionAttachment attachment)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.EncryptionName))
            {
                return string.Empty;
            }

            return Winform4System.Business.Services.ShareDataStorage.Download(Path.Combine(EnsureInspectionPhotoFolder(batchMaterialId), attachment.EncryptionName));
        }

        public static void DeletePhotoFile(int batchMaterialId, SparePartInspectionAttachment attachment)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.EncryptionName)) return;
            string path = Path.Combine(EnsureInspectionPhotoFolder(batchMaterialId), attachment.EncryptionName);
            Winform4System.Business.Services.ShareDataStorage.Delete(path);
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
