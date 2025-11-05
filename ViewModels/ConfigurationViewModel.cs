using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DesktopCompanions.Models;
using DesktopCompanions.Services;

namespace DesktopCompanions.ViewModels
{
    /// <summary>
    /// ViewModel for the Configuration window
    /// </summary>
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly PerformanceMonitorService _performanceMonitor;

        public ConfigurationViewModel(SettingsService settingsService, PerformanceMonitorService performanceMonitor)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _performanceMonitor = performanceMonitor ?? throw new ArgumentNullException(nameof(performanceMonitor));

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
                if (_runOnStartup != value)
                {
                    _runOnStartup = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _alwaysOnTop;
        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                if (_alwaysOnTop != value)
                {
                    _alwaysOnTop = value;
                    OnPropertyChanged();
                }
            }
        }

        // Quantum Cat Settings
        private bool _enableQuantumCat;
        public bool EnableQuantumCat
        {
            get => _enableQuantumCat;
            set
            {
                if (_enableQuantumCat != value)
                {
                    _enableQuantumCat = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _catSizeScale;
        public double CatSizeScale
        {
            get => _catSizeScale;
            set
            {
                if (Math.Abs(_catSizeScale - value) > 0.01)
                {
                    _catSizeScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _catCpuIdleThreshold;
        public double CatCpuIdleThreshold
        {
            get => _catCpuIdleThreshold;
            set
            {
                if (Math.Abs(_catCpuIdleThreshold - value) > 0.01)
                {
                    _catCpuIdleThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _catCpuMediumThreshold;
        public double CatCpuMediumThreshold
        {
            get => _catCpuMediumThreshold;
            set
            {
                if (Math.Abs(_catCpuMediumThreshold - value) > 0.01)
                {
                    _catCpuMediumThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _catCpuHighThreshold;
        public double CatCpuHighThreshold
        {
            get => _catCpuHighThreshold;
            set
            {
                if (Math.Abs(_catCpuHighThreshold - value) > 0.01)
                {
                    _catCpuHighThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        private Point _catPosition;
        public Point CatPosition
        {
            get => _catPosition;
            set
            {
                if (_catPosition != value)
                {
                    _catPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        // Goblin Jar Settings
        private bool _enableGoblinJar;
        public bool EnableGoblinJar
        {
            get => _enableGoblinJar;
            set
            {
                if (_enableGoblinJar != value)
                {
                    _enableGoblinJar = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _jarSizeScale;
        public double JarSizeScale
        {
            get => _jarSizeScale;
            set
            {
                if (Math.Abs(_jarSizeScale - value) > 0.01)
                {
                    _jarSizeScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _jarLowBatteryThreshold;
        public double JarLowBatteryThreshold
        {
            get => _jarLowBatteryThreshold;
            set
            {
                if (Math.Abs(_jarLowBatteryThreshold - value) > 0.01)
                {
                    _jarLowBatteryThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _jarCriticalBatteryThreshold;
        public double JarCriticalBatteryThreshold
        {
            get => _jarCriticalBatteryThreshold;
            set
            {
                if (Math.Abs(_jarCriticalBatteryThreshold - value) > 0.01)
                {
                    _jarCriticalBatteryThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        private Point _jarPosition;
        public Point JarPosition
        {
            get => _jarPosition;
            set
            {
                if (_jarPosition != value)
                {
                    _jarPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Load settings from SettingsService
        /// </summary>
        public void LoadSettings()
        {
            var settings = _settingsService.Settings;

            // General Settings
            RunOnStartup = settings.RunOnStartup;
            AlwaysOnTop = settings.AlwaysOnTop;

            // Quantum Cat Settings
            EnableQuantumCat = settings.EnableQuantumCat;
            CatSizeScale = settings.CatSizeScale;
            CatCpuIdleThreshold = settings.CatCpuIdleThreshold;
            CatCpuMediumThreshold = settings.CatCpuMediumThreshold;
            CatCpuHighThreshold = settings.CatCpuHighThreshold;
            CatPosition = settings.CatPosition;

            // Goblin Jar Settings
            EnableGoblinJar = settings.EnableGoblinJar;
            JarSizeScale = settings.JarSizeScale;
            JarLowBatteryThreshold = settings.JarLowBatteryThreshold;
            JarCriticalBatteryThreshold = settings.JarCriticalBatteryThreshold;
            JarPosition = settings.JarPosition;
        }

        /// <summary>
        /// Save settings to SettingsService and update registry
        /// </summary>
        public void SaveSettings()
        {
            var settings = _settingsService.Settings;

            // General Settings
            settings.RunOnStartup = RunOnStartup;
            settings.AlwaysOnTop = AlwaysOnTop;

            // Quantum Cat Settings
            settings.EnableQuantumCat = EnableQuantumCat;
            settings.CatSizeScale = CatSizeScale;
            settings.CatCpuIdleThreshold = CatCpuIdleThreshold;
            settings.CatCpuMediumThreshold = CatCpuMediumThreshold;
            settings.CatCpuHighThreshold = CatCpuHighThreshold;
            settings.CatPosition = CatPosition;

            // Goblin Jar Settings
            settings.EnableGoblinJar = EnableGoblinJar;
            settings.JarSizeScale = JarSizeScale;
            settings.JarLowBatteryThreshold = JarLowBatteryThreshold;
            settings.JarCriticalBatteryThreshold = JarCriticalBatteryThreshold;
            settings.JarPosition = JarPosition;

            // Persist to disk
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

