using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DesktopCompanions.Services;
using DesktopCompanions.ViewModels;
using DesktopCompanion; // WindowUtils is in DesktopCompanion namespace

namespace DesktopCompanions
{
    /// <summary>
    /// Interaction logic for QuantumCatWidget.xaml
    /// </summary>
    public partial class QuantumCatWidget : Window
    {
        private readonly CatWidgetViewModel _viewModel;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private DispatcherTimer? _animationTimer;
        private int _currentFrame = 0;
        private const int TotalFrames = 16;
        private const int FrameWidth = 32;

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

            // Load sprite sheet image
            LoadSpriteSheet();

            // Start sprite sheet animation
            StartSpriteAnimation();
        }

        private void LoadSpriteSheet()
        {
            try
            {
                // Try pack URI first (works in normal builds)
                var uri = new Uri("pack://application:,,,/Assets/cat_running_spritesheet.png", UriKind.Absolute);
                var bitmap = new System.Windows.Media.Imaging.BitmapImage(uri);
                CatImage.Source = bitmap;
            }
            catch
            {
                // Fallback: try loading from file system (for single-file apps)
                try
                {
                    var appDir = AppContext.BaseDirectory;
                    var imagePath = System.IO.Path.Combine(appDir, "Assets", "cat_running_spritesheet.png");
                    if (System.IO.File.Exists(imagePath))
                    {
                        var uri = new Uri(imagePath, UriKind.Absolute);
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage(uri);
                        CatImage.Source = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load sprite sheet: {ex.Message}");
                }
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

        private void StartSpriteAnimation()
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
            // Update sprite sheet frame
            _currentFrame = (_currentFrame + 1) % TotalFrames;
            
            // Update clip rectangle to show current frame
            if (SpriteClip != null)
            {
                var xOffset = _currentFrame * FrameWidth;
                SpriteClip.Rect = new Rect(xOffset, 0, FrameWidth, FrameWidth);
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
            base.OnClosed(e);
        }
    }
}
