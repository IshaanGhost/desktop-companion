using System;
using System.Windows;
using DesktopCompanions.Services;
using DesktopCompanions.ViewModels;

namespace DesktopCompanions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Configuration window for Desktop Companions
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigurationViewModel _viewModel;
        private readonly App _app;

        public MainWindow(PerformanceMonitorService performanceMonitor, SettingsService settingsService)
        {
            InitializeComponent();

            if (performanceMonitor == null)
                throw new ArgumentNullException(nameof(performanceMonitor));
            if (settingsService == null)
                throw new ArgumentNullException(nameof(settingsService));

            _app = (App)Application.Current;
            _viewModel = new ConfigurationViewModel(settingsService, performanceMonitor);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings
            _viewModel.SaveSettings();

            // Refresh widgets in the application
            _app?.RefreshWidgets();

            // Close window
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Reload settings to discard changes
            _viewModel.LoadSettings();

            // Close window
            this.Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Hide instead of closing when user clicks X
            e.Cancel = true;
            this.Hide();
        }
    }
}
