using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DesktopCompanions.Services;

namespace DesktopCompanions.ViewModels
{
    /// <summary>
    /// ViewModel for the Quantum Cat widget
    /// Binds to PerformanceMonitorService.CpuLoad to determine animation state
    /// </summary>
    public class CatWidgetViewModel : INotifyPropertyChanged
    {
        private readonly PerformanceMonitorService _performanceMonitor;
        private CatAnimationState _currentState = CatAnimationState.Idle;

        public CatWidgetViewModel(PerformanceMonitorService performanceMonitor)
        {
            _performanceMonitor = performanceMonitor ?? throw new ArgumentNullException(nameof(performanceMonitor));
            
            // Subscribe to PropertyChanged events from PerformanceMonitorService
            _performanceMonitor.PropertyChanged += OnPerformanceMonitorPropertyChanged;
            
            // Initialize state based on current CPU load
            UpdateState(_performanceMonitor.CpuLoad);
        }

        private CatAnimationState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(AnimationStateName));
                    OnPropertyChanged(nameof(AnimationSpeed));
                    OnPropertyChanged(nameof(CatColor));
                }
            }
        }

        /// <summary>
        /// Animation state name for binding to XAML DataTriggers
        /// </summary>
        public string AnimationStateName => CurrentState.ToString();

        /// <summary>
        /// Animation speed multiplier (0.5 = slow, 3.0 = very fast)
        /// </summary>
        public double AnimationSpeed
        {
            get
            {
                return CurrentState switch
                {
                    CatAnimationState.Idle => 0.5,      // Slow animation
                    CatAnimationState.Medium => 1.0,     // Normal speed
                    CatAnimationState.High => 2.0,       // Fast
                    CatAnimationState.HighLoad => 3.0,   // Very fast
                    _ => 1.0
                };
            }
        }

        /// <summary>
        /// Cat color based on load state (for visual feedback)
        /// </summary>
        public string CatColor
        {
            get
            {
                return CurrentState switch
                {
                    CatAnimationState.Idle => "#FF4CAF50",      // Green - idle
                    CatAnimationState.Medium => "#FFFF9800",    // Orange - medium
                    CatAnimationState.High => "#FFFF5722",       // Deep Orange - high
                    CatAnimationState.HighLoad => "#FFE91E63",  // Pink - critical
                    _ => "#FF4CAF50"
                };
            }
        }

        /// <summary>
        /// Current CPU load (exposed for binding)
        /// </summary>
        public double CpuLoad => _performanceMonitor.CpuLoad;

        private void OnPerformanceMonitorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PerformanceMonitorService.CpuLoad))
            {
                UpdateState(_performanceMonitor.CpuLoad);
                OnPropertyChanged(nameof(CpuLoad));
            }
        }

        private void UpdateState(double cpuLoad)
        {
            // Map CPU load to visual state
            // Default thresholds: Idle < 20%, Medium < 60%, High < 90%, HighLoad >= 90%
            CatAnimationState newState = cpuLoad switch
            {
                < 20.0 => CatAnimationState.Idle,
                < 60.0 => CatAnimationState.Medium,
                < 90.0 => CatAnimationState.High,
                _ => CatAnimationState.HighLoad
            };

            CurrentState = newState;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CatAnimationState
    {
        Idle,      // < 20% CPU
        Medium,    // 20-60% CPU
        High,      // 60-90% CPU
        HighLoad   // >= 90% CPU
    }
}
