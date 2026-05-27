using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class StatsService
    {
        private readonly SecurityLogger _logger;
        private readonly AuthService _auth;

        public StatsService() : this(new AuthService()) { }

        public StatsService(AuthService auth)
        {
            _logger = new SecurityLogger();
            _auth = auth;
        }

        private void RequireAuth()
        {
            var session = _auth.GetCurrentSession();
            if (!session.IsActive)
                throw new InvalidOperationException("Not logged in. Use --login first.");
            if (session.Role != "admin" && session.Role != "watcher")
                throw new InvalidOperationException("Admin or watcher role required for monitoring.");
        }

        public (double cpu, double ramUsedGb, double ramTotalGb, double ramPercent,
                List<(string name, double totalGb, double usedGb, double percent)> disks) GetStats()
        {
            RequireAuth();
            var cpu = GetCpuUsage();
            var (totalMb, freeMb) = GetRamInfo();
            var totalGb = Math.Round(totalMb / 1024.0, 1);
            var usedGb = Math.Round((totalMb - freeMb) / 1024.0, 1);
            var ramPercent = totalMb > 0 ? Math.Round((totalMb - freeMb) / totalMb * 100, 1) : 0;

            var disks = GetDiskInfo();

            _logger.Log(_auth.GetCurrentSession().Username, "stats", "success",
                $"CPU:{cpu}% RAM:{ramPercent}%");

            return (cpu, usedGb, totalGb, ramPercent, disks);
        }

        private static double GetCpuUsage()
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'");
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["PercentProcessorTime"]);
            }
            return 0;
        }

        private static (double totalMb, double freeMb) GetRamInfo()
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                var total = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                var free = Convert.ToDouble(obj["FreePhysicalMemory"]);
                return (total / 1024.0, free / 1024.0);
            }
            return (0, 0);
        }

        private static List<(string name, double totalGb, double usedGb, double percent)> GetDiskInfo()
        {
            var disks = new List<(string name, double totalGb, double usedGb, double percent)>();
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
            {
                var total = drive.TotalSize / (1024.0 * 1024 * 1024);
                var free = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                var used = total - free;
                var percent = total > 0 ? Math.Round(used / total * 100, 1) : 0;
                disks.Add((drive.Name.TrimEnd('\\'), Math.Round(total, 1), Math.Round(used, 1), percent));
            }
            return disks;
        }
    }
}
