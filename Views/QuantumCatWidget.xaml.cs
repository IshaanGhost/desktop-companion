using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
    /// Interaction logic for QuantumCatWidget.xaml
    /// </summary>
    public partial class QuantumCatWidget : Window
    {
        private readonly CatWidgetViewModel _viewModel;
        private bool _isDragging = false;
        private System.Windows.Point _dragStartPoint;
        private DispatcherTimer? _animationTimer;
        private int _currentFrame = 0;
        private const int CatFrameCount = 8; // 8 cat animation frames
        private Icon[]? _catIcons;

        public QuantumCatWidget(PerformanceMonitorService performanceMonitor)
        {
            InitializeComponent();

            if (performanceMonitor == null)
                throw new ArgumentNullException(nameof(performanceMonitor));

            // Create ViewModel and set as DataContext
            _viewModel = new CatWidgetViewModel(performanceMonitor);
            DataContext = _viewModel;

            // Subscribe to property changes to update animation speed
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Set up click-through behavior (except for drag handle)
            // Uses Win32 API via WindowUtils
            SetClickThrough();

            // Set window properties
            Topmost = true;

            // Load cat icons
            LoadCatIcons();

            // Start icon animation
            
            // Set tooltip DataContext after window is loaded
            Loaded += (s, e) =>
            {
                if (CatIconImage?.ToolTip is ToolTip tooltip)
                {
                    tooltip.DataContext = _viewModel;
                }
            };
            
            StartIconAnimation();
        }

        private void LoadCatIcons()
        {
            try
            {
                _catIcons = new Icon[CatFrameCount];
                for (int i = 0; i < CatFrameCount; i++)
                {
                    _catIcons[i] = CreateCatIcon(i + 1);
                }
                // Display first frame
                UpdateCatIcon(0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load cat icons: {ex.Message}");
            }
        }

        private Icon CreateCatIcon(int frameNumber)
        {
            // Create a 48x48 icon showing a running cat - slightly bigger for widget display
            using (var bitmap = new Bitmap(48, 48))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                var catColor = Color.FromArgb(100, 100, 100);
                var catDark = Color.White; // White borders

                // Animation offset based on frame
                int offsetX = (frameNumber % 2) * 2;
                int offsetY = (frameNumber % 4 < 2) ? 0 : 2;

                // Cat head (scaled up)
                graphics.FillEllipse(new SolidBrush(catColor), 18 + offsetX, 6 + offsetY, 18, 18);
                graphics.DrawEllipse(new Pen(catDark, 2), 18 + offsetX, 6 + offsetY, 18, 18);

                // Cat ears (larger triangles)
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(21 + offsetX, 6 + offsetY), new Point(24 + offsetX, 0), new Point(27 + offsetX, 6 + offsetY)
                });
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(27 + offsetX, 6 + offsetY), new Point(30 + offsetX, 0), new Point(33 + offsetX, 6 + offsetY)
                });

                // Eyes (keep black for visibility)
                graphics.FillEllipse(new SolidBrush(Color.Black), 22 + offsetX, 12 + offsetY, 3, 3);
                graphics.FillEllipse(new SolidBrush(Color.Black), 29 + offsetX, 12 + offsetY, 3, 3);

                // Cat body
                graphics.FillEllipse(new SolidBrush(catColor), 21 + offsetX, 21 + offsetY, 15, 15);
                graphics.DrawEllipse(new Pen(catDark, 2), 21 + offsetX, 21 + offsetY, 15, 15);

                // Cat tail (swings based on frame)
                int tailOffset = (frameNumber % 3) - 1;
                graphics.FillRectangle(new SolidBrush(catColor), 36 + offsetX + tailOffset * 2, 24 + offsetY, 6, 9);
                graphics.DrawRectangle(new Pen(catDark, 2), 36 + offsetX + tailOffset * 2, 24 + offsetY, 6, 9);

                // Convert bitmap to icon
                var hIcon = bitmap.GetHicon();
                return System.Drawing.Icon.FromHandle(hIcon);
            }
        }

        private void UpdateCatIcon(int frameIndex)
        {
            if (_catIcons == null || frameIndex < 0 || frameIndex >= _catIcons.Length) return;
            
            try
            {
                var icon = _catIcons[frameIndex];
                var bitmapImage = Utils.IconConverter.IconToBitmapImage(icon, 48);
                CatIconImage.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update cat icon: {ex.Message}");
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.AnimationStateName))
            {
                // Update animation speed based on state
                UpdateAnimationSpeed();
            }
        }

        private void StartIconAnimation()
        {
            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += AnimationTimer_Tick;
            UpdateAnimationSpeed();
            _animationTimer.Start();
        }

        private void UpdateAnimationSpeed()
        {
            if (_animationTimer == null) return;

            // Adjust animation speed based on CPU load state
            var state = _viewModel.AnimationStateName;
            var interval = state switch
            {
                "Idle" => TimeSpan.FromMilliseconds(200),      // Slow for idle
                "Medium" => TimeSpan.FromMilliseconds(100),    // Medium speed
                "High" => TimeSpan.FromMilliseconds(60),       // Fast for high load
                "HighLoad" => TimeSpan.FromMilliseconds(40),   // Very fast for critical
                _ => TimeSpan.FromMilliseconds(150)
            };

            _animationTimer.Interval = interval;
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            // Update cat icon frame
            _currentFrame = (_currentFrame + 1) % CatFrameCount;
            UpdateCatIcon(_currentFrame);
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
                
                // Capture initial mouse position and window position
                var mousePos = e.GetPosition(this);
                _dragStartPoint = mousePos;
                
                // Store initial window position
                var initialLeft = Left;
                var initialTop = Top;
                
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
                
                // Get current window position (before any changes)
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
            _animationTimer?.Stop();
            _animationTimer = null;

            // Dispose icons
            if (_catIcons != null)
            {
                foreach (var icon in _catIcons)
                {
                    icon?.Dispose();
                }
            }

            base.OnClosed(e);
        }
    }
}
