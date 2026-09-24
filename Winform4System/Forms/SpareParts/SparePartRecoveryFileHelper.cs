using Winform4System.DataAccess.Entities.SpareParts;
using Winform4System.Forms.SpareParts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Winform4System.Forms.SpareParts
{
    public static class SparePartRecoveryFileHelper
    {
        private const string RecoveryGuideFolder = "RecoveryGuides";
        private const string RecoveryEvidenceFolder = "RecoveryEvidence";

        public static string EnsureBaseFolder()
        {
            if (!Directory.Exists(SparePartConfiguration.SparePartFolder))
            {
                Directory.CreateDirectory(SparePartConfiguration.SparePartFolder);
            }

            return SparePartConfiguration.SparePartFolder;
        }

        public static string EnsureGuideFolder()
        {
            string folder = Path.Combine(EnsureBaseFolder(), RecoveryGuideFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static string EnsureEvidenceFolder(int ticketId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), RecoveryEvidenceFolder, ticketId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName, string extension) SaveGuideFile(string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = SparePartEncryptionHelper.EncryptionFileName(sourceFilePath);
            string extension = Path.GetExtension(sourceFilePath) ?? string.Empty;
            File.Copy(sourceFilePath, Path.Combine(EnsureGuideFolder(), encryptionName), true);
            return (encryptionName, actualName, extension);
        }

        public static (string encryptionName, string actualName, string extension) SaveEvidenceFile(int ticketId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = SparePartEncryptionHelper.EncryptionFileName(sourceFilePath);
            string extension = Path.GetExtension(sourceFilePath) ?? string.Empty;
            File.Copy(sourceFilePath, Path.Combine(EnsureEvidenceFolder(ticketId), encryptionName), true);
            return (encryptionName, actualName, extension);
        }

        public static string GetGuidePath(SparePartRecoveryGuide guide)
        {
            return Path.Combine(EnsureGuideFolder(), guide.EncryptionName);
        }

        public static string GetEvidencePath(SparePartRecoveryEvidence evidence)
        {
            return Path.Combine(EnsureEvidenceFolder(evidence.RecoveryTicketId), evidence.EncryptionName);
        }

        public static void OpenGuideFiles(IEnumerable<SparePartRecoveryGuide> guides)
        {
            if (guides == null)
            {
                return;
            }

            OpenStoredFiles(guides.Select(guide => (guide.ActualName, GetGuidePath(guide))));
        }

        public static void OpenEvidenceFiles(IEnumerable<SparePartRecoveryEvidence> evidences)
        {
            if (evidences == null)
            {
                return;
            }

            OpenStoredFiles(evidences.Select(evidence => (evidence.ActualName, GetEvidencePath(evidence))));
        }

        public static void OpenLocalFiles(IEnumerable<string> files)
        {
            if (files == null)
            {
                return;
            }

            OpenStoredFiles(files
                .Where(File.Exists)
                .Select(file => (Path.GetFileName(file), file)));
        }

        private static void OpenStoredFiles(IEnumerable<(string actualName, string physicalPath)> files)
        {
            var validFiles = files
                .Where(item => !string.IsNullOrWhiteSpace(item.actualName) && !string.IsNullOrWhiteSpace(item.physicalPath))
                .Where(item => File.Exists(item.physicalPath))
                .ToList();

            if (validFiles.Count == 0)
            {
                MessageBox.Show("找不到對應檔案。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var item in validFiles)
            {
                string tempFile = SparePartHelper.CopyToTemp(item.physicalPath, item.actualName);
                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
        }
    }
}
