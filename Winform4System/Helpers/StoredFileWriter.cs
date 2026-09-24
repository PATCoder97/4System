using System;
using System.IO;

namespace Winform4System.Helpers
{
    public static class StoredFileWriter
    {
        private const int MaxAttempts = 5;

        public static string CopyToStorage(string sourceFilePath, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("Source file path is required.", nameof(sourceFilePath));
            if (string.IsNullOrWhiteSpace(destinationFolder))
                throw new ArgumentException("Destination folder is required.", nameof(destinationFolder));

            Directory.CreateDirectory(destinationFolder);

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                string storedName = StoredFileNameGenerator.Create();
                string destinationPath = Path.Combine(destinationFolder, storedName);

                try
                {
                    File.Copy(sourceFilePath, destinationPath, false);
                    return storedName;
                }
                catch (IOException)
                {
                    if (!File.Exists(destinationPath))
                        throw;
                }
            }

            throw new IOException("Unable to allocate a unique stored file name.");
        }
    }
}
