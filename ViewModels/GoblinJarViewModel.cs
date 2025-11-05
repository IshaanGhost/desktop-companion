using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using DesktopCompanions.Services;

namespace DesktopCompanions.ViewModels
{
    /// <summary>
    /// ViewModel for the Goblin Jar widget
    /// Binds to PerformanceMonitorService.BatteryLifePercent and BatteryStatus
    /// </summary>
    public class JarWidgetViewModel : INotifyPropertyChanged
    {
        private readonly PerformanceMonitorService _performanceMonitor;
        private JarAnimationState _currentState = JarAnimationState.High;

        public JarWidgetViewModel(PerformanceMonitorService performanceMonitor)
        {
            _performanceMonitor = performanceMonitor ?? throw new ArgumentNullException(nameof(performanceMonitor));
            
            // Subscribe to PropertyChanged events from PerformanceMonitorService
            _performanceMonitor.PropertyChanged += OnPerformanceMonitorPropertyChanged;
            
            // Initialize state based on current battery status
            UpdateState(_performanceMonitor.BatteryLifePercent, _performanceMonitor.BatteryStatus);
        }

        private JarAnimationState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(AnimationStateName));
                    OnPropertyChanged(nameof(AnimationSpeed));
                    OnPropertyChanged(nameof(GlowIntensity));
                    OnPropertyChanged(nameof(GlowColor));
                    OnPropertyChanged(nameof(IsCharging));
                }
            }
        }

        /// <summary>
        /// Animation state name for binding to XAML DataTriggers
        /// </summary>
        public string AnimationStateName => CurrentState.ToString();

        /// <summary>
        /// Animation speed multiplier
        /// </summary>
        public double AnimationSpeed
        {
            get
            {
                return CurrentState switch
                {
                    JarAnimationState.High => 1.0,
                    JarAnimationState.Medium => 0.8,
                    JarAnimationState.Low => 0.5,
                    JarAnimationState.Critical => 2.0,  // Fast flashing
                    JarAnimationState.Charging => 2.0,
                    JarAnimationState.Full => 0.3,
                    _ => 1.0
                };
            }
        }

        /// <summary>
        /// Glow intensity based on battery level (0.0 - 1.0)
        /// </summary>
        public double GlowIntensity
        {
            get
            {
                return CurrentState switch
                {
                    JarAnimationState.High => 1.0,
                    JarAnimationState.Medium => 0.6,
                    JarAnimationState.Low => 0.3,
                    JarAnimationState.Critical => 0.8,  // Red glow
                    JarAnimationState.Charging => 0.9,
                    JarAnimationState.Full => 1.0,
                    _ => 1.0
                };
            }
        }

        /// <summary>
        /// Glow color based on battery state and level
        /// </summary>
        public string GlowColor
        {
            get
            {
                return CurrentState switch
                {
                    JarAnimationState.High => "#FFFFFF00",      // Yellow - high battery
                    JarAnimationState.Medium => "#FFFFA500",   // Orange - medium
                    JarAnimationState.Low => "#FFFF8C00",       // Dark Orange - low
                    JarAnimationState.Critical => "#FFFF0000",  // Red - critical
                    JarAnimationState.Charging => "#FF00FF00",  // Green - charging
                    JarAnimationState.Full => "#FF00FFFF",      // Cyan - full
                    _ => "#FFFFFF00"
                };
            }
        }

        /// <summary>
        /// Whether the battery is currently charging
        /// </summary>
        public bool IsCharging => CurrentState == JarAnimationState.Charging;

        /// <summary>
        /// Current battery life percentage (0-100)
        /// </summary>
        public float BatteryLifePercent => _performanceMonitor.BatteryLifePercent;

        /// <summary>
        /// Current battery status (PowerLineStatus enum)
        /// </summary>
        public PowerLineStatus BatteryStatus => _performanceMonitor.BatteryStatus;

        /// <summary>
        /// Battery status as readable text
        /// </summary>
        public string BatteryStatusText
        {
            get
            {
                return BatteryStatus switch
                {
                    PowerLineStatus.Online => "Charging",
                    PowerLineStatus.Offline => "On Battery",
                    PowerLineStatus.Unknown => "Unknown",
                    _ => "Not Present"
                };
            }
        }

        private void OnPerformanceMonitorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PerformanceMonitorService.BatteryLifePercent) ||
                e.PropertyName == nameof(PerformanceMonitorService.BatteryStatus))
            {
                UpdateState(_performanceMonitor.BatteryLifePercent, _performanceMonitor.BatteryStatus);
                OnPropertyChanged(nameof(BatteryLifePercent));
                OnPropertyChanged(nameof(BatteryStatus));
                OnPropertyChanged(nameof(BatteryStatusText));
            }
        }

        private void UpdateState(float batteryPercent, PowerLineStatus batteryStatus)
        {
            // Map battery status and level to visual state
            // Default thresholds: Critical < 10%, Low < 20%, Medium < 70%, High >= 70%
            JarAnimationState newState;

            if (batteryStatus == PowerLineStatus.Online)
            {
                // Charging
                if (batteryPercent >= 100.0f)
                {
                    newState = JarAnimationState.Full;
                }
                else
                {
                    newState = JarAnimationState.Charging;
                }
            }
            else if (batteryStatus == PowerLineStatus.Offline)
            {
                // On battery
                newState = batteryPercent switch
                {
                    < 10.0f => JarAnimationState.Critical,
                    < 20.0f => JarAnimationState.Low,
                    < 70.0f => JarAnimationState.Medium,
                    _ => JarAnimationState.High
                };
            }
            else
            {
                // Unknown/NotPresent - default to High for desktops
                newState = JarAnimationState.High;
            }

            CurrentState = newState;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum JarAnimationState
    {
        High,      // 70-100%
        Medium,    // 20-70%
        Low,       // 10-20%
        Critical,  // < 10%
        Charging,  // Charging
        Full       // Full and plugged in
    }
}
