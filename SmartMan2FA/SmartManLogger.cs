using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMan2FA
{
    internal class SmartManLogger
    {
        private readonly string logDirectory;
        public SmartManLogger(string logDirectory)
        {
            string logFileName = $"smartman2fa_{DateTime.Now:yyyyMMdd}.txt";
            if (string.IsNullOrEmpty(logDirectory))
            {
                this.logDirectory = logFileName;
            }
            else
            {
                // Check if the logDirectory is a fully qualified path
                if (!Path.IsPathFullyQualified(logDirectory))
                {
                    // Combine with the base directory
                    logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logDirectory);
                }

                // Ensure the directory exists
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                this.logDirectory = Path.Combine(logDirectory, logFileName);
            }
        }
        public void LogText(string message)
        {
            string formattedMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} --- {message}";
            Console.WriteLine(formattedMessage);
            using var logFile = new StreamWriter(logDirectory, true);
            logFile.WriteLine(formattedMessage);
        }
        public void LogError(string message, Exception? ex = null)
        {
            string formattedMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} --- [Error] {message}";
            if (ex != null)
            {
                formattedMessage += $" {ex.Message}, [StackTrace] {ex.StackTrace}";
            }
            Console.WriteLine(formattedMessage);
            using var logFile = new StreamWriter(logDirectory, true);
            logFile.WriteLine(formattedMessage);
        }
        // Delete old log files
        public void DeleteOldLogs(int days)
        {
            // Get the directory path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string? logFolder = Path.GetDirectoryName(Path.Combine(baseDirectory, this.logDirectory));

            // If the directory path is not fully qualified, and the directory does not exist, return
            if (logFolder == null || !Path.IsPathFullyQualified(logFolder) && !Directory.Exists(logFolder))
            {
                return;
            }
            var files = Directory.GetFiles(logFolder).Where(f => f.StartsWith("encryptor_log_"));
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < DateTime.Now.AddDays(-days))
                {
                    fileInfo.Delete();
                }
            }
        }
    }
}
