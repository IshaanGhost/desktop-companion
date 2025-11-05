using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using DesktopCompanions.Models;

namespace DesktopCompanions.Services
{
    /// <summary>
    /// Service for managing application settings persistence
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsPath;
        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = Path.Combine(appDataPath, "DesktopCompanion");
            
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            _settingsPath = Path.Combine(settingsDir, "settings.json");
            Settings = new AppSettings();
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // If loading fails, use default settings
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
                Settings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}
