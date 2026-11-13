using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Winform4System.Business.Services
{
    public static class ShareDataStorage
    {
        private static readonly Lazy<HttpClient> Client = new Lazy<HttpClient>(CreateClient);
        public static string EnvironmentName
        {
            get
            {
                string value = ConfigurationManager.AppSettings["AppEnvironment"] ?? "Test";
                if (value != "Test" && value != "Prod")
                    throw new InvalidOperationException("執行環境必須為 Test 或 Prod。");
                return value.ToLowerInvariant();
            }
        }
        public static string CacheRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Winform4System", "FileCache", EnvironmentName);

        private static HttpClient CreateClient()
        {
            string url = ConfigurationManager.AppSettings["ShareDataApiBaseUrl"];
            string key = ConfigurationManager.AppSettings["ShareDataApiKey"];
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("尚未設定檔案 API 位址或存取金鑰。");
            var client = new HttpClient { BaseAddress = new Uri(url.TrimEnd('/') + "/"), Timeout = TimeSpan.FromMinutes(5) };
            client.DefaultRequestHeaders.Add("X-API-Key", key);
            return client;
        }

        private static string FileUrl(string cachePath)
        {
            string root = Path.GetFullPath(CacheRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string path = Path.GetFullPath(cachePath);
            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("檔案路徑超出儲存範圍。");
            string relative = path.Substring(root.Length).Replace('\\', '/');
            return "api/files/winform4system/" + EnvironmentName + "/"
                + string.Join("/", relative.Split('/').Select(Uri.EscapeDataString));
        }

        public static void Upload(string sourcePath, string cachePath)
        {
            UploadAsync(sourcePath, FileUrl(cachePath)).GetAwaiter().GetResult();
        }

        private static async Task UploadAsync(string sourcePath, string url)
        {
            using (var stream = File.OpenRead(sourcePath))
            using (var content = new StreamContent(stream))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                using (var response = await Client.Value.PutAsync(url + "?overwrite=false", content).ConfigureAwait(false))
                    EnsureSuccess(response);
            }
        }

        public static string Download(string cachePath)
        {
            DownloadAsync(FileUrl(cachePath), cachePath).GetAwaiter().GetResult();
            return cachePath;
        }

        private static async Task DownloadAsync(string url, string cachePath)
        {
            using (var response = await Client.Value.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                EnsureSuccess(response);
                Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
                string temporary = cachePath + "." + Guid.NewGuid().ToString("N") + ".download";
                try
                {
                    using (var output = File.Create(temporary))
                        await response.Content.CopyToAsync(output).ConfigureAwait(false);
                    File.Copy(temporary, cachePath, true);
                }
                finally { if (File.Exists(temporary)) File.Delete(temporary); }
            }
        }

        private static void EnsureSuccess(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new FileNotFoundException("伺服器上找不到指定檔案。");
            if (!response.IsSuccessStatusCode)
                throw new IOException("檔案服務處理失敗，狀態碼：" + (int)response.StatusCode);
        }

        public static async Task CheckHealthAsync()
        {
            using (var response = await Client.Value.GetAsync("api/files?path="
                + Uri.EscapeDataString("winform4system/" + EnvironmentName) + "&limit=1").ConfigureAwait(false))
            {
                // A fresh environment has no files yet.
                if (response.StatusCode != HttpStatusCode.NotFound) EnsureSuccess(response);
            }
        }
    }
}
