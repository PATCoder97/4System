using System;
using System.IO;
using System.Text;

namespace Winform4System.Logging
{
    public sealed class FileAppLogger : IAppLogger
    {
        private static readonly object SyncRoot = new object();
        private readonly string _logDirectory;

        public FileAppLogger(string logDirectory = null)
        {
            _logDirectory = logDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        public void Info(string source, string message) => Write("INFO", source, message, null);

        public void Error(string source, string message, Exception exception = null) => Write("ERROR", source, message, exception);

        private void Write(string level, string source, string message, Exception exception)
        {
            try
            {
                Directory.CreateDirectory(_logDirectory);
                string filePath = Path.Combine(_logDirectory, $"app-{DateTime.Today:yyyy-MM-dd}.log");
                var line = new StringBuilder()
                    .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                    .Append(" | ").Append(level)
                    .Append(" | ").Append(source ?? "Application")
                    .Append(" | ").Append(message ?? string.Empty);

                if (exception != null)
                    line.AppendLine().Append(exception);

                lock (SyncRoot)
                    File.AppendAllText(filePath, line.AppendLine().ToString(), Encoding.UTF8);
            }
            catch
            {
                // Logging must never prevent the application from starting.
            }
        }
    }
}
