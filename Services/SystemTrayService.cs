using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;

namespace DesktopCompanions.Services
{
    /// <summary>
    /// Service for managing system tray icon with dynamic icon swapping
    /// Implements cat running animation and goblin battery status icons
    /// </summary>
    public class SystemTrayService : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private bool _disposed = false;
        private readonly PerformanceMonitorService _performanceMonitor;
        private DispatcherTimer? _iconAnimationTimer;
        private int _currentFrame = 0;
        private const int CatFrameCount = 8; // Cat_Run_01 to Cat_Run_08
        private Icon? _currentIcon;
        private Icon? _goblinChargingIcon;
        private Icon? _goblinLowIcon;
        private Icon[]? _catIcons;

        public event EventHandler? ShowConfiguration;
        public event EventHandler? ExitApplication;

        public SystemTrayService(PerformanceMonitorService performanceMonitor)
        {
            _performanceMonitor = performanceMonitor ?? throw new ArgumentNullException(nameof(performanceMonitor));
            
            // Subscribe to property changes
            _performanceMonitor.PropertyChanged += PerformanceMonitor_PropertyChanged;
        }

        public void Initialize()
        {
            // Load icons
            LoadIcons();

            // Create NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Icon = _catIcons?[0] ?? CreateDefaultIcon(),
                Text = "Desktop Companions - CPU: 0% | Battery: 100%",
                Visible = true
            };

            // Update tooltip when metrics change
            _performanceMonitor.PropertyChanged += (s, e) => UpdateTooltip();

            // Create context menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, OnShowConfiguration);
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Exit", null, OnExitApplication);
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += OnShowConfiguration;

            // Start icon animation timer
            StartIconAnimation();
        }

        private void LoadIcons()
        {
            try
            {
                // Load cat running animation frames (8 frames)
                _catIcons = new Icon[CatFrameCount];
                for (int i = 0; i < CatFrameCount; i++)
                {
                    _catIcons[i] = CreateCatIcon(i + 1);
                }

                // Create goblin icons
                _goblinChargingIcon = CreateGoblinChargingIcon();
                _goblinLowIcon = CreateGoblinLowIcon();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load icons: {ex.Message}");
                // Fallback to default icon
            }
        }

        private Icon CreateCatIcon(int frameNumber)
        {
            // Create a 16x16 icon showing a running cat - simplified for visibility
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.None; // Pixel-perfect for small icons
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.Clear(Color.Transparent);

                var catColor = Color.FromArgb(100, 100, 100); // Medium gray for better visibility
                var catDark = Color.Black;

                // Animation offset based on frame (creates running effect)
                int offsetX = (frameNumber % 2) * 1;
                int offsetY = (frameNumber % 4 < 2) ? 0 : 1;

                // Simplified cat - larger shapes for visibility
                // Cat head (larger, simpler)
                graphics.FillEllipse(new SolidBrush(catColor), 6 + offsetX, 1 + offsetY, 6, 6);
                graphics.DrawEllipse(new Pen(catDark, 1), 6 + offsetX, 1 + offsetY, 6, 6);

                // Cat ears (larger triangles)
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(7 + offsetX, 1 + offsetY), new Point(8 + offsetX, 0), new Point(9 + offsetX, 1 + offsetY)
                });
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(9 + offsetX, 1 + offsetY), new Point(10 + offsetX, 0), new Point(11 + offsetX, 1 + offsetY)
                });

                // Cat body (larger, simpler)
                graphics.FillEllipse(new SolidBrush(catColor), 7 + offsetX, 7 + offsetY, 5, 5);
                graphics.DrawEllipse(new Pen(catDark, 1), 7 + offsetX, 7 + offsetY, 5, 5);

                // Cat tail (visible movement)
                int tailOffset = (frameNumber % 3) - 1;
                graphics.FillRectangle(new SolidBrush(catColor), 12 + offsetX + tailOffset, 8 + offsetY, 2, 3);
                graphics.DrawRectangle(new Pen(catDark, 1), 12 + offsetX + tailOffset, 8 + offsetY, 2, 3);

                var hIcon = bitmap.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        private Icon CreateGoblinChargingIcon()
        {
            // Create icon showing goblin in jar with charging effect - larger for visibility
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.Clear(Color.Transparent);

                // Jar outline - larger and more visible
                var jarBlue = Color.FromArgb(90, 160, 245);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(120, jarBlue)), 2, 1, 7, 12);
                graphics.DrawRectangle(new Pen(jarBlue, 2), 2, 1, 7, 12);

                // Charging goblin (larger green circle)
                var chargingGreen = Color.FromArgb(0, 255, 136);
                graphics.FillEllipse(new SolidBrush(chargingGreen), 3, 5, 5, 5);
                graphics.DrawEllipse(new Pen(Color.White, 1), 3, 5, 5, 5);
                
                // Charging sparkles (more visible)
                graphics.FillEllipse(new SolidBrush(Color.Yellow), 4, 3, 3, 3);
                graphics.FillEllipse(new SolidBrush(Color.Yellow), 6, 7, 2, 2);

                var hIcon = bitmap.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        private Icon CreateGoblinLowIcon()
        {
            // Create icon showing goblin in jar with low battery effect - larger for visibility
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.Clear(Color.Transparent);

                // Jar outline - larger and more visible
                var jarBlue = Color.FromArgb(90, 160, 245);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(120, jarBlue)), 2, 1, 7, 12);
                graphics.DrawRectangle(new Pen(jarBlue, 2), 2, 1, 7, 12);

                // Low battery goblin (larger, more visible orange/red)
                var lowColor = Color.FromArgb(255, 100, 0);
                graphics.FillEllipse(new SolidBrush(lowColor), 3, 5, 5, 5);
                // Warning effect - red border
                graphics.DrawEllipse(new Pen(Color.Red, 2), 3, 5, 5, 5);

                var hIcon = bitmap.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        private Icon CreateDefaultIcon()
        {
            // Fallback default icon
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.FillEllipse(new SolidBrush(Color.Gray), 4, 4, 8, 8);
                var hIcon = bitmap.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        private void StartIconAnimation()
        {
            _iconAnimationTimer = new DispatcherTimer();
            _iconAnimationTimer.Tick += IconAnimationTimer_Tick;
            UpdateAnimationInterval();
            _iconAnimationTimer.Start();
        }

        private void UpdateAnimationInterval()
        {
            if (_iconAnimationTimer == null) return;

            // Get current CPU load to determine animation speed
            double cpuLoad = _performanceMonitor.CpuLoad;
            
            // Calculate interval based on CPU load (faster CPU = faster animation)
            // Range: 200ms (idle) to 40ms (high load)
            int intervalMs = cpuLoad switch
            {
                < 20 => 200,      // Idle: slow
                < 60 => 100,      // Medium: normal
                < 90 => 60,       // High: fast
                _ => 40           // Critical: very fast
            };

            _iconAnimationTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
        }

        private void IconAnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (_notifyIcon == null || _disposed) return;

            // Check battery status first (goblin icons override cat animation)
            var batteryStatus = _performanceMonitor.BatteryStatus;
            var batteryPercent = _performanceMonitor.BatteryLifePercent;

            // Condition 1: Charging - show goblin charging icon
            if (batteryStatus == System.Windows.Forms.PowerLineStatus.Online)
            {
                if (_currentIcon != _goblinChargingIcon && _goblinChargingIcon != null)
                {
                    _currentIcon?.Dispose();
                    _currentIcon = _goblinChargingIcon;
                    _notifyIcon.Icon = _goblinChargingIcon;
                    // Show notification for charging
                    _notifyIcon.ShowBalloonTip(2000, "Battery Charging", 
                        $"Battery is charging ({batteryPercent:F0}%)", 
                        ToolTipIcon.Info);
                }
                return;
            }

            // Condition 2: Low battery (< 20%) and discharging - show goblin low icon
            if (batteryPercent < 20 && batteryStatus == System.Windows.Forms.PowerLineStatus.Offline)
            {
                if (_currentIcon != _goblinLowIcon && _goblinLowIcon != null)
                {
                    _currentIcon?.Dispose();
                    _currentIcon = _goblinLowIcon;
                    _notifyIcon.Icon = _goblinLowIcon;
                    // Show notification for low battery
                    if (batteryPercent < 10)
                    {
                        _notifyIcon.ShowBalloonTip(3000, "Low Battery Warning", 
                            $"Battery critically low ({batteryPercent:F0}%)!", 
                            ToolTipIcon.Warning);
                    }
                }
                return;
            }

            // Condition 3: Normal/High - show cat animation
            if (_catIcons != null && _catIcons.Length > 0)
            {
                _currentFrame = (_currentFrame + 1) % _catIcons.Length;
                var nextIcon = _catIcons[_currentFrame];
                
                if (_currentIcon != nextIcon)
                {
                    _currentIcon?.Dispose();
                    _currentIcon = nextIcon;
                    _notifyIcon.Icon = nextIcon;
                }
            }
        }

        private void PerformanceMonitor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Update animation interval when CPU load changes
            if (e.PropertyName == nameof(PerformanceMonitorService.CpuLoad))
            {
                UpdateAnimationInterval();
                UpdateTooltip();
            }
            else if (e.PropertyName == nameof(PerformanceMonitorService.BatteryLifePercent) ||
                     e.PropertyName == nameof(PerformanceMonitorService.BatteryStatus))
            {
                UpdateTooltip();
            }
        }

        private void UpdateTooltip()
        {
            if (_notifyIcon == null) return;

            var cpu = _performanceMonitor.CpuLoad;
            var battery = _performanceMonitor.BatteryLifePercent;
            var batteryStatus = _performanceMonitor.BatteryStatus;

            string batteryText = batteryStatus == System.Windows.Forms.PowerLineStatus.Online 
                ? $"Charging ({battery:F0}%)" 
                : $"{battery:F0}%";

            _notifyIcon.Text = $"Desktop Companions\nCPU: {cpu:F1}%\nBattery: {batteryText}";
        }

        private void OnShowConfiguration(object? sender, EventArgs e)
        {
            ShowConfiguration?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitApplication(object? sender, EventArgs e)
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _iconAnimationTimer?.Stop();
            _iconAnimationTimer = null;

            _currentIcon?.Dispose();
            
            if (_catIcons != null)
            {
                foreach (var icon in _catIcons)
                {
                    icon?.Dispose();
                }
            }

            _goblinChargingIcon?.Dispose();
            _goblinLowIcon?.Dispose();

            _notifyIcon?.Dispose();
            
            if (_performanceMonitor != null)
            {
                _performanceMonitor.PropertyChanged -= PerformanceMonitor_PropertyChanged;
            }

            _disposed = true;
        }
    }
}
