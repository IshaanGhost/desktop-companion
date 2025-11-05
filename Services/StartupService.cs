using Microsoft.Win32;
using System;

namespace DesktopCompanions.Services
{
    /// <summary>
    /// Service for managing Windows startup registration
    /// </summary>
    public class StartupService
    {
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "DesktopCompanion";

        public static void SetStartup(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
                if (key == null) return;

                if (enable)
                {
                    // For single-file apps, use Environment.ProcessPath
                    // Assembly.Location returns empty string in single-file apps
                    var exePath = Environment.ProcessPath;
                    if (string.IsNullOrEmpty(exePath))
                    {
                        // Fallback: use AppContext.BaseDirectory + executable name
                        var appDir = AppContext.BaseDirectory;
                        var exeName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
                        exePath = System.IO.Path.Combine(appDir, exeName);
                    }
                    key.SetValue(AppName, exePath);
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set startup: {ex.Message}");
            }
        }

        public static bool IsStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
                if (key == null) return false;

                return key.GetValue(AppName) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
