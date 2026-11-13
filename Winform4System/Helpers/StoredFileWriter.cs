using System;
using System.IO;
using Winform4System.Business.Services;
namespace Winform4System.Helpers
{
    public static class StoredFileWriter
    {
        public static string CopyToStorage(string sourceFilePath, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("請選擇來源檔案。", nameof(sourceFilePath));
            string storedName = StoredFileNameGenerator.Create();
            ShareDataStorage.Upload(sourceFilePath, Path.Combine(destinationFolder, storedName));
            return storedName;
        }
    }
}
