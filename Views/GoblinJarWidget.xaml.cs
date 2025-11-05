using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopCompanions.Services;
using DesktopCompanions.ViewModels;
using DesktopCompanions.Utils;
using DesktopCompanion; // WindowUtils is in DesktopCompanion namespace
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace DesktopCompanions
{
    /// <summary>
    /// Interaction logic for GoblinJarWidget.xaml
    /// </summary>
    public partial class GoblinJarWidget : Window
    {
        private readonly JarWidgetViewModel _viewModel;
        private bool _isDragging = false;
        private System.Windows.Point _dragStartPoint;
        private Icon? _currentJarIcon;

        public GoblinJarWidget(PerformanceMonitorService performanceMonitor)
        {
            InitializeComponent();

            if (performanceMonitor == null)
                throw new ArgumentNullException(nameof(performanceMonitor));

            // Create ViewModel and set as DataContext
            _viewModel = new JarWidgetViewModel(performanceMonitor);
            DataContext = _viewModel;

            // Subscribe to property changes to update icon
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Set up click-through behavior (except for drag handle)
            // Uses Win32 API via WindowUtils
            SetClickThrough();

            // Set window properties
            Topmost = true;

            // Load initial jar icon
            UpdateJarIcon();
            
            // Set tooltip DataContext after window is loaded
            Loaded += (s, e) =>
            {
                if (JarIconImage?.ToolTip is ToolTip tooltip)
                {
                    tooltip.DataContext = _viewModel;
                }
            };
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.AnimationStateName))
            {
                UpdateJarIcon();
            }
        }

        private void UpdateJarIcon()
        {
            var state = _viewModel.AnimationStateName;
            var batteryPercent = _viewModel.BatteryLifePercent;
            var batteryStatus = _viewModel.BatteryStatus;

            Icon? icon = null;

            // Determine which icon to show based on state
            if (batteryStatus == System.Windows.Forms.PowerLineStatus.Online)
            {
                // Charging
                icon = CreateGoblinChargingIcon();
            }
            else if (batteryPercent < 20)
            {
                // Low battery
                icon = CreateGoblinLowIcon();
            }
            else
            {
                // Normal - show based on battery level
                icon = CreateGoblinNormalIcon(batteryPercent);
            }

            if (icon != null)
            {
                _currentJarIcon?.Dispose();
                _currentJarIcon = icon;
                var bitmapImage = Utils.IconConverter.IconToBitmapImage(icon, 40);
                JarIconImage.Source = bitmapImage;
            }
        }

        private Icon CreateGoblinChargingIcon()
        {
            // Create icon showing goblin in jar with charging effect - half size, transparent
            using (var bitmap = new Bitmap(40, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                // Jar outline - glass jar with rounded top (transparent, no fill)
                var jarBlue = Color.FromArgb(90, 160, 245);
                var jarPath = new GraphicsPath();
                jarPath.AddLine(15, 25, 15, 45); // Left side
                jarPath.AddLine(15, 45, 25, 45); // Bottom
                jarPath.AddLine(25, 45, 25, 25); // Right side
                jarPath.AddArc(15, 20, 10, 10, 180, 180); // Rounded top
                jarPath.CloseFigure();

                // Only draw outline, no fill (transparent)
                graphics.DrawPath(new Pen(jarBlue, 2), jarPath);

                // Charging goblin (smaller green circle)
                var chargingGreen = Color.FromArgb(0, 255, 136);
                graphics.FillEllipse(new SolidBrush(chargingGreen), 16, 31, 8, 8);
                graphics.DrawEllipse(new Pen(Color.White, 1), 16, 31, 8, 8);

                // Charging sparkles (smaller)
                graphics.FillEllipse(new SolidBrush(Color.Yellow), 18, 26, 4, 4);
                graphics.FillEllipse(new SolidBrush(Color.Yellow), 21, 35, 3, 3);

                var hIcon = bitmap.GetHicon();
                return System.Drawing.Icon.FromHandle(hIcon);
            }
        }

        private Icon CreateGoblinLowIcon()
        {
            // Create icon showing goblin in jar with low battery effect - half size, transparent
            using (var bitmap = new Bitmap(40, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                // Jar outline (transparent, no fill)
                var jarBlue = Color.FromArgb(90, 160, 245);
                var jarPath = new GraphicsPath();
                jarPath.AddLine(15, 25, 15, 45);
                jarPath.AddLine(15, 45, 25, 45);
                jarPath.AddLine(25, 45, 25, 25);
                jarPath.AddArc(15, 20, 10, 10, 180, 180);
                jarPath.CloseFigure();

                // Only draw outline, no fill (transparent)
                graphics.DrawPath(new Pen(jarBlue, 2), jarPath);

                // Low battery goblin (smaller orange/red)
                var lowColor = Color.FromArgb(255, 100, 0);
                graphics.FillEllipse(new SolidBrush(lowColor), 16, 31, 8, 8);
                graphics.DrawEllipse(new Pen(Color.Red, 2), 16, 31, 8, 8);

                var hIcon = bitmap.GetHicon();
                return System.Drawing.Icon.FromHandle(hIcon);
            }
        }

        private Icon CreateGoblinNormalIcon(float batteryPercent)
        {
            // Create icon showing goblin in jar - color varies by battery level, transparent
            using (var bitmap = new Bitmap(40, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                // Jar outline (transparent, no fill)
                var jarBlue = Color.FromArgb(90, 160, 245);
                var jarPath = new GraphicsPath();
                jarPath.AddLine(15, 25, 15, 45);
                jarPath.AddLine(15, 45, 25, 45);
                jarPath.AddLine(25, 45, 25, 25);
                jarPath.AddArc(15, 20, 10, 10, 180, 180);
                jarPath.CloseFigure();

                // Only draw outline, no fill (transparent)
                graphics.DrawPath(new Pen(jarBlue, 2), jarPath);

                // Goblin color based on battery level
                Color goblinColor;
                if (batteryPercent > 80)
                    goblinColor = Color.FromArgb(0, 255, 255); // Cyan for full
                else if (batteryPercent > 50)
                    goblinColor = Color.FromArgb(0, 255, 136); // Green for good
                else if (batteryPercent > 30)
                    goblinColor = Color.FromArgb(255, 165, 0); // Orange for medium
                else
                    goblinColor = Color.FromArgb(255, 140, 0); // Dark orange for low

                graphics.FillEllipse(new SolidBrush(goblinColor), 16, 31, 8, 8);
                graphics.DrawEllipse(new Pen(Color.White, 1), 16, 31, 8, 8);

                var hIcon = bitmap.GetHicon();
                return System.Drawing.Icon.FromHandle(hIcon);
            }
        }

        private void SetClickThrough()
        {
            // Set window to be click-through except for the drag handle area
            // This uses Win32 API calls (implemented via P/Invoke in WindowUtils)
            WindowUtils.SetClickThrough(this);
        }

        private void DragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                
                // Capture initial mouse position
                var mousePos = e.GetPosition(this);
                _dragStartPoint = mousePos;
                
                DragHandle.CaptureMouse();
                
                // Ensure window is visible and active
                Visibility = Visibility.Visible;
                Show();
                Activate();
                
                // Temporarily disable click-through during drag
                WindowUtils.DisableClickThrough(this);
            }
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                // Get current mouse position relative to window
                var currentPos = e.GetPosition(this);
                
                // Calculate offset from initial drag point
                var deltaX = currentPos.X - _dragStartPoint.X;
                var deltaY = currentPos.Y - _dragStartPoint.Y;
                
                // Get current window position
                var currentLeft = Left;
                var currentTop = Top;
                
                // Calculate new window position
                var newLeft = currentLeft + deltaX;
                var newTop = currentTop + deltaY;

                // Keep widget within screen bounds
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                
                newLeft = Math.Max(0, Math.Min(newLeft, screenWidth - Width));
                newTop = Math.Max(0, Math.Min(newTop, screenHeight - Height));

                // Update window position
                Left = newLeft;
                Top = newTop;
                
                // Reset drag start point to current position for next iteration
                _dragStartPoint = currentPos;
                
                // Ensure window stays visible
                Visibility = Visibility.Visible;
                Show();
            }
        }

        private void DragHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                DragHandle.ReleaseMouseCapture();
                
                // Re-enable click-through after drag
                WindowUtils.SetClickThrough(this);
                
                // Ensure window stays visible
                Visibility = Visibility.Visible;
                Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _currentJarIcon?.Dispose();
            base.OnClosed(e);
        }
    }
}
