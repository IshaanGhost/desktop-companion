using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
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
            // Create a 32x32 icon showing a running cat - half size for widget display
            using (var bitmap = new Bitmap(32, 32))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                var catColor = Color.FromArgb(100, 100, 100);
                var catDark = Color.Black;

                // Animation offset based on frame
                int offsetX = (frameNumber % 2) * 1;
                int offsetY = (frameNumber % 4 < 2) ? 0 : 1;

                // Cat head (scaled down for half size)
                graphics.FillEllipse(new SolidBrush(catColor), 12 + offsetX, 4 + offsetY, 12, 12);
                graphics.DrawEllipse(new Pen(catDark, 1), 12 + offsetX, 4 + offsetY, 12, 12);

                // Cat ears (smaller triangles)
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(14 + offsetX, 4 + offsetY), new Point(16 + offsetX, 0), new Point(18 + offsetX, 4 + offsetY)
                });
                graphics.FillPolygon(new SolidBrush(catDark), new Point[] {
                    new Point(18 + offsetX, 4 + offsetY), new Point(20 + offsetX, 0), new Point(22 + offsetX, 4 + offsetY)
                });

                // Eyes
                graphics.FillEllipse(new SolidBrush(Color.Black), 15 + offsetX, 8 + offsetY, 2, 2);
                graphics.FillEllipse(new SolidBrush(Color.Black), 19 + offsetX, 8 + offsetY, 2, 2);

                // Cat body
                graphics.FillEllipse(new SolidBrush(catColor), 14 + offsetX, 14 + offsetY, 10, 10);
                graphics.DrawEllipse(new Pen(catDark, 1), 14 + offsetX, 14 + offsetY, 10, 10);

                // Cat tail (swings based on frame)
                int tailOffset = (frameNumber % 3) - 1;
                graphics.FillRectangle(new SolidBrush(catColor), 24 + offsetX + tailOffset, 16 + offsetY, 4, 6);
                graphics.DrawRectangle(new Pen(catDark, 1), 24 + offsetX + tailOffset, 16 + offsetY, 4, 6);

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
                var bitmapImage = Utils.IconConverter.IconToBitmapImage(icon, 32);
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
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this);
            DragHandle.CaptureMouse();
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(null);
                var newLeft = currentPoint.X - _dragStartPoint.X;
                var newTop = currentPoint.Y - _dragStartPoint.Y;

                // Keep widget within screen bounds
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                
                newLeft = Math.Max(0, Math.Min(newLeft, screenWidth - Width));
                newTop = Math.Max(0, Math.Min(newTop, screenHeight - Height));

                Left = newLeft;
                Top = newTop;
            }
        }

        private void DragHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DragHandle.ReleaseMouseCapture();
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
