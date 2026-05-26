using System;
using System.IO;

namespace ConsoleApp15.Services
{
    public class SecurityLogger
    {
        private readonly string _logPath;

        public SecurityLogger()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dir);
            _logPath = Path.Combine(dir, "security.log");
        }

        public void Log(string username, string action, string status, string details = "")
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {username,-15} | {action,-20} | {status,-8} | {details}";
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }

        public string[] ReadAll()
        {
            if (!File.Exists(_logPath))
                return Array.Empty<string>();

            return File.ReadAllLines(_logPath);
        }
    }
}
