using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DesktopCompanions.Models;
using DesktopCompanions.Services;

namespace DesktopCompanions.ViewModels
{
    /// <summary>
    /// Main ViewModel for the configuration window
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly PerformanceMonitorService _performanceMonitor;

        public MainViewModel(SettingsService settingsService, PerformanceMonitorService performanceMonitor)
        {
            _settingsService = settingsService;
            _performanceMonitor = performanceMonitor;

            // Load settings
            LoadSettings();
        }

        // General Settings
        private bool _runOnStartup;
        public bool RunOnStartup
        {
            get => _runOnStartup;
            set
            {
                _runOnStartup = value;
                OnPropertyChanged();
            }
        }

        private bool _alwaysOnTop;
        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                _alwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        // Quantum Cat Settings
        private bool _enableQuantumCat;
        public bool EnableQuantumCat
        {
            get => _enableQuantumCat;
            set
            {
                _enableQuantumCat = value;
                OnPropertyChanged();
            }
        }

        private double _catSizeScale;
        public double CatSizeScale
        {
            get => _catSizeScale;
            set
            {
                _catSizeScale = value;
                OnPropertyChanged();
            }
        }

        private double _catCpuIdleThreshold;
        public double CatCpuIdleThreshold
        {
            get => _catCpuIdleThreshold;
            set
            {
                _catCpuIdleThreshold = value;
                OnPropertyChanged();
            }
        }

        private double _catCpuMediumThreshold;
        public double CatCpuMediumThreshold
        {
            get => _catCpuMediumThreshold;
            set
            {
                _catCpuMediumThreshold = value;
                OnPropertyChanged();
            }
        }

        private double _catCpuHighThreshold;
        public double CatCpuHighThreshold
        {
            get => _catCpuHighThreshold;
            set
            {
                _catCpuHighThreshold = value;
                OnPropertyChanged();
            }
        }

        // Goblin Jar Settings
        private bool _enableGoblinJar;
        public bool EnableGoblinJar
        {
            get => _enableGoblinJar;
            set
            {
                _enableGoblinJar = value;
                OnPropertyChanged();
            }
        }

        private double _jarSizeScale;
        public double JarSizeScale
        {
            get => _jarSizeScale;
            set
            {
                _jarSizeScale = value;
                OnPropertyChanged();
            }
        }

        private double _jarLowBatteryThreshold;
        public double JarLowBatteryThreshold
        {
            get => _jarLowBatteryThreshold;
            set
            {
                _jarLowBatteryThreshold = value;
                OnPropertyChanged();
            }
        }

        private double _jarCriticalBatteryThreshold;
        public double JarCriticalBatteryThreshold
        {
            get => _jarCriticalBatteryThreshold;
            set
            {
                _jarCriticalBatteryThreshold = value;
                OnPropertyChanged();
            }
        }

        public void LoadSettings()
        {
            var settings = _settingsService.Settings;
            RunOnStartup = settings.RunOnStartup;
            AlwaysOnTop = settings.AlwaysOnTop;
            EnableQuantumCat = settings.EnableQuantumCat;
            CatSizeScale = settings.CatSizeScale;
            CatCpuIdleThreshold = settings.CatCpuIdleThreshold;
            CatCpuMediumThreshold = settings.CatCpuMediumThreshold;
            CatCpuHighThreshold = settings.CatCpuHighThreshold;
            EnableGoblinJar = settings.EnableGoblinJar;
            JarSizeScale = settings.JarSizeScale;
            JarLowBatteryThreshold = settings.JarLowBatteryThreshold;
            JarCriticalBatteryThreshold = settings.JarCriticalBatteryThreshold;
        }

        public void SaveSettings()
        {
            var settings = _settingsService.Settings;
            settings.RunOnStartup = RunOnStartup;
            settings.AlwaysOnTop = AlwaysOnTop;
            settings.EnableQuantumCat = EnableQuantumCat;
            settings.CatSizeScale = CatSizeScale;
            settings.CatCpuIdleThreshold = CatCpuIdleThreshold;
            settings.CatCpuMediumThreshold = CatCpuMediumThreshold;
            settings.CatCpuHighThreshold = CatCpuHighThreshold;
            settings.EnableGoblinJar = EnableGoblinJar;
            settings.JarSizeScale = JarSizeScale;
            settings.JarLowBatteryThreshold = JarLowBatteryThreshold;
            settings.JarCriticalBatteryThreshold = JarCriticalBatteryThreshold;

            _settingsService.SaveSettings();

            // Update startup registry
            StartupService.SetStartup(RunOnStartup);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
