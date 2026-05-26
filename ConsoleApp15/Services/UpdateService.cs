using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Web.Script.Serialization;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class UpdateService
    {
        private readonly HttpClient _http;
        private readonly SecurityLogger _logger;
        private readonly AuthService _auth;
        private const string ReleasesUrl = "https://api.github.com/repos/she1en/notes/releases/latest";

        public UpdateService()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent", "notes-cli");
            _logger = new SecurityLogger();
            _auth = new AuthService();
        }

        public (bool available, string version, string url) CheckForUpdates()
        {
            try
            {
                var json = _http.GetStringAsync(ReleasesUrl).Result;
                var serializer = new JavaScriptSerializer();
                var release = serializer.Deserialize<GitHubRelease>(json);

                if (release == null || string.IsNullOrEmpty(release.tag_name))
                    return (false, "", "");

                var tagVersion = release.tag_name.TrimStart('v', 'V');
                var currentVersion = AppVersion.Version;

                if (tagVersion == currentVersion)
                    return (false, tagVersion, "");

                var downloadUrl = "";
                if (release.assets != null && release.assets.Length > 0)
                    downloadUrl = release.assets[0].browser_download_url;

                return (true, tagVersion, downloadUrl);
            }
            catch
            {
                return (false, "", "");
            }
        }

        public (bool success, string message) ApplyUpdate(string downloadUrl)
        {
            var session = _auth.GetCurrentSession();
            if (!session.IsActive)
                return (false, "Not logged in.");

            if (string.IsNullOrEmpty(downloadUrl))
                return (false, "No update URL available. Run --checkUpdate first.");

            try
            {
                var currentExe = Process.GetCurrentProcess().MainModule.FileName;
                var dir = Path.GetDirectoryName(currentExe);
                var updateExe = Path.Combine(dir, "update_temp.exe");
                var backupExe = Path.Combine(dir, "ConsoleApp15_old.exe");

                var data = _http.GetByteArrayAsync(downloadUrl).Result;
                File.WriteAllBytes(updateExe, data);

                var scriptPath = Path.Combine(Path.GetTempPath(), "update_notes.bat");
                var script = $@"
@echo off
timeout /t 2 /nobreak >nul
copy /y ""{updateExe}"" ""{currentExe}"" >nul
del ""{updateExe}""
start """" ""{currentExe}""
del ""%~f0""
";
                File.WriteAllText(scriptPath, script);

                var psi = new ProcessStartInfo
                {
                    FileName = scriptPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true
                };
                Process.Start(psi);

                _logger.Log(session.Username, "update", "success", $"version={AppVersion.Version}");
                return (true, "Update downloaded. Restarting...");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public void CheckOnStartup()
        {
            try
            {
                var json = _http.GetStringAsync(ReleasesUrl).Result;
                var serializer = new JavaScriptSerializer();
                var release = serializer.Deserialize<GitHubRelease>(json);

                if (release == null || string.IsNullOrEmpty(release.tag_name))
                    return;

                var tagVersion = release.tag_name.TrimStart('v', 'V');
                if (tagVersion != AppVersion.Version)
                {
                    Console.WriteLine($"Update available: v{tagVersion}. Use --checkUpdate for details.");
                }
            }
            catch
            {
            }
        }

        private class GitHubRelease
        {
            public string tag_name { get; set; }
            public string name { get; set; }
            public GitHubAsset[] assets { get; set; }
        }

        private class GitHubAsset
        {
            public string name { get; set; }
            public string browser_download_url { get; set; }
        }
    }
}
