using System.ComponentModel;
using System.Runtime.CompilerServices;
using DesktopCompanions.Services;

namespace DesktopCompanions.ViewModels
{
    /// <summary>
    /// ViewModel for the MainWindow displaying CPU load
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PerformanceMonitorService _performanceMonitorService;

        public MainWindowViewModel(PerformanceMonitorService performanceMonitorService)
        {
            _performanceMonitorService = performanceMonitorService;
            
            // Subscribe to property changes from the service
            _performanceMonitorService.PropertyChanged += PerformanceMonitorService_PropertyChanged;
        }

        private void PerformanceMonitorService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // When CpuLoad changes in the service, update our property
            if (e.PropertyName == nameof(PerformanceMonitorService.CpuLoad))
            {
                OnPropertyChanged(nameof(CpuLoad));
            }
        }

        /// <summary>
        /// CPU Load percentage from the performance monitor service
        /// </summary>
        public double CpuLoad => _performanceMonitorService.CpuLoad;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

