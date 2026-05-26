using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public string[] ReadFiltered(DateTime? from = null, DateTime? to = null, string username = null)
        {
            var lines = ReadAll();
            if (lines.Length == 0)
                return lines;

            var filtered = lines.Where(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                if (line.Length < 19)
                    return false;

                if (DateTime.TryParse(line.Substring(0, 19), out var lineDate))
                {
                    if (from.HasValue && lineDate < from.Value)
                        return false;
                    if (to.HasValue && lineDate > to.Value)
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    var parts = line.Split('|');
                    if (parts.Length < 2)
                        return false;
                    var logUser = parts[1].Trim();
                    if (!logUser.Equals(username, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return true;
            }).ToArray();

            return filtered;
        }
    }
}
