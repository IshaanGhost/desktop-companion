using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopCompanions.Services
{
    /// <summary>
    /// Service for monitoring system performance metrics (CPU, RAM, Battery)
    /// Uses PerformanceCounter API and System.Windows.Forms.PowerStatus
    /// Implements INotifyPropertyChanged for real-time property updates
    /// </summary>
    public class PerformanceMonitorService : INotifyPropertyChanged, IDisposable
    {
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private System.Threading.Timer? _monitoringTimer;
        private bool _disposed = false;

        private double _cpuLoad;
        private double _ramUsagePercent;
        private float _batteryLifePercent;
        private PowerLineStatus _batteryStatus;

        /// <summary>
        /// CPU Load percentage (% Processor Time)
        /// </summary>
        public double CpuLoad
        {
            get => _cpuLoad;
            private set
            {
                if (_cpuLoad != value)
                {
                    _cpuLoad = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// RAM Usage percentage (% Committed Bytes In Use)
        /// </summary>
        public double RamUsagePercent
        {
            get => _ramUsagePercent;
            private set
            {
                if (_ramUsagePercent != value)
                {
                    _ramUsagePercent = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Battery Life percentage (0-100)
        /// </summary>
        public float BatteryLifePercent
        {
            get => _batteryLifePercent;
            private set
            {
                if (_batteryLifePercent != value)
                {
                    _batteryLifePercent = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Battery Status (PowerLineStatus enum)
        /// </summary>
        public PowerLineStatus BatteryStatus
        {
            get => _batteryStatus;
            private set
            {
                if (_batteryStatus != value)
                {
                    _batteryStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Starts monitoring system metrics using a background timer
        /// </summary>
        public void StartMonitoring()
        {
            try
            {
                // Initialize CPU counter (% Processor Time)
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // First call returns 0, so we skip it

                // Initialize RAM counter (% Committed Bytes In Use)
                _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                _ramCounter.NextValue(); // First call returns 0

                // Start timer to poll metrics every 500ms
                _monitoringTimer = new System.Threading.Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize performance counters: {ex.Message}");
            }
        }

        private void UpdateMetrics(object? state)
        {
            if (_disposed) return;

            try
            {
                // Update CPU usage
                if (_cpuCounter != null)
                {
                    CpuLoad = Math.Min(100.0, Math.Max(0.0, _cpuCounter.NextValue()));
                }

                // Update RAM usage (% Committed Bytes In Use)
                if (_ramCounter != null)
                {
                    RamUsagePercent = Math.Min(100.0, Math.Max(0.0, _ramCounter.NextValue()));
                }

                // Update battery status using System.Windows.Forms.PowerStatus
                UpdateBatteryStatus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating metrics: {ex.Message}");
            }
        }

        private void UpdateBatteryStatus()
        {
            try
            {
                var powerStatus = SystemInformation.PowerStatus;

                // BatteryLifePercent (0-100)
                BatteryLifePercent = powerStatus.BatteryLifePercent * 100.0f;

                // BatteryStatus (PowerLineStatus enum)
                BatteryStatus = powerStatus.PowerLineStatus;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get battery status: {ex.Message}");
                // Default values if battery info is not available
                BatteryLifePercent = 100.0f;
                BatteryStatus = PowerLineStatus.Unknown;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _monitoringTimer?.Dispose();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();

            _disposed = true;
        }
    }
}
