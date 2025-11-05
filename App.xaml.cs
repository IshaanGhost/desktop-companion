using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using DesktopCompanions.Services;
using DesktopCompanions.ViewModels;

namespace DesktopCompanions
{
    public partial class App : Application
    {
        private PerformanceMonitorService? _performanceMonitor;
        private SettingsService? _settingsService;
        private SystemTrayService? _systemTrayService;
        private MainWindow? _configurationWindow;
        private QuantumCatWidget? _catWidget;
        private GoblinJarWidget? _jarWidget;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set up global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Dispatcher.UnhandledExceptionFilter += OnDispatcherUnhandledExceptionFilter;

            try
            {
                // Initialize services
                _settingsService = new SettingsService();
                _settingsService.LoadSettings();

                _performanceMonitor = new PerformanceMonitorService();
                _performanceMonitor.StartMonitoring();

                // Initialize system tray with icon animation
                try
                {
                    _systemTrayService = new SystemTrayService(_performanceMonitor);
                    _systemTrayService.Initialize();
                    _systemTrayService.ShowConfiguration += OnShowConfiguration;
                    _systemTrayService.ExitApplication += OnExitApplication;
                }
                catch (Exception ex)
                {
                    LogError($"Failed to initialize system tray: {ex}");
                    MessageBox.Show(
                        $"Failed to initialize system tray.\n\nError: {ex.Message}",
                        "Startup Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                // Create configuration window (hidden by default, only shown when requested)
                // Don't create it on startup - only create when user opens settings

                // Load widgets - show cat and jar on desktop
                LoadWidgets();
            }
            catch (Exception ex)
            {
                LogError($"Critical error during startup: {ex}");
                MessageBox.Show(
                    $"Failed to start Desktop Companion.\n\nError: {ex.Message}\n\nCheck the error log for details.",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // This is now handled in OnStartup
        }

        private void LoadWidgets()
        {
            if (_performanceMonitor == null) return;

            // Get screen dimensions and taskbar height
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
            var workAreaBottom = SystemParameters.WorkArea.Bottom;

            try
            {
                // Load Quantum Cat Widget
                if (_catWidget == null)
                {
                    _catWidget = new QuantumCatWidget(_performanceMonitor);
                    _catWidget.Closed += (s, args) => { _catWidget = null; };
                    
                    // Position right above taskbar (bottom-right corner)
                    _catWidget.Left = screenWidth - _catWidget.Width - 20;
                    _catWidget.Top = workAreaBottom - _catWidget.Height - 10; // 10px above taskbar
                }
                _catWidget.Show();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load Quantum Cat widget: {ex}");
            }

            try
            {
                // Load Goblin Jar Widget
                if (_jarWidget == null)
                {
                    _jarWidget = new GoblinJarWidget(_performanceMonitor);
                    _jarWidget.Closed += (s, args) => { _jarWidget = null; };
                    
                    // Position right above taskbar (to the left of cat)
                    var catWidth = _catWidget?.Width ?? 100;
                    _jarWidget.Left = screenWidth - _jarWidget.Width - catWidth - 30;
                    _jarWidget.Top = workAreaBottom - _jarWidget.Height - 10; // 10px above taskbar, aligned with cat
                }
                _jarWidget.Show();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load Goblin Jar widget: {ex}");
            }
        }


        private void OnShowConfiguration(object? sender, EventArgs e)
        {
            if (_configurationWindow == null)
            {
                if (_performanceMonitor == null || _settingsService == null) return;
                _configurationWindow = new MainWindow(_performanceMonitor, _settingsService);
                _configurationWindow.Closed += (s, args) => { _configurationWindow = null; };
            }

            _configurationWindow.Show();
            _configurationWindow.Activate();
            _configurationWindow.WindowState = WindowState.Normal;
        }

        private void OnExitApplication(object? sender, EventArgs e)
        {
            // Save settings before exit
            _settingsService?.SaveSettings();

            // Close all windows
            _catWidget?.Close();
            _jarWidget?.Close();
            _configurationWindow?.Close();

            // Dispose resources
            _systemTrayService?.Dispose();
            _performanceMonitor?.Dispose();

            // Explicitly shutdown
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Cleanup
            _systemTrayService?.Dispose();
            _performanceMonitor?.Dispose();
        }


        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception ?? new Exception("Unknown exception");
            LogError($"Unhandled exception: {exception}");
            
            MessageBox.Show(
                $"An unexpected error occurred.\n\nError: {exception.Message}\n\nCheck the error log for details.",
                "Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private static bool _errorDialogShown = false;
        
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogError($"Dispatcher unhandled exception: {e.Exception}");
            e.Handled = true; // Prevent app from crashing
            
            // Only show one error dialog to prevent spam
            if (!_errorDialogShown)
            {
                _errorDialogShown = true;
                try
                {
                    MessageBox.Show(
                        $"An error occurred in the UI thread.\n\nError: {e.Exception.Message}\n\nCheck the error log for details.\n\nFurther errors will be logged but not displayed.",
                        "UI Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                finally
                {
                    // Reset after a delay to allow showing again if needed
                    System.Threading.Tasks.Task.Delay(5000).ContinueWith(_ => _errorDialogShown = false);
                }
            }
        }

        private void OnDispatcherUnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            LogError($"Dispatcher exception filter: {e.Exception}");
            e.RequestCatch = true; // Catch the exception
        }

        private void LogError(string message)
        {
            try
            {
                var logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DesktopCompanion",
                    "error.log");
                
                var logDir = Path.GetDirectoryName(logPath);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // If logging fails, at least write to debug output
                System.Diagnostics.Debug.WriteLine($"Logging failed. Original error: {message}");
            }
        }
    }
}
