using System;
using System.Windows;
using System.Windows.Input;
using DesktopCompanions.Services;
using DesktopCompanions.ViewModels;
using DesktopCompanion; // WindowUtils is in DesktopCompanion namespace

namespace DesktopCompanions
{
    /// <summary>
    /// Interaction logic for GoblinJarWidget.xaml
    /// </summary>
    public partial class GoblinJarWidget : Window
    {
        private readonly JarWidgetViewModel _viewModel;
        private bool _isDragging = false;
        private Point _dragStartPoint;

        public GoblinJarWidget(PerformanceMonitorService performanceMonitor)
        {
            InitializeComponent();

            if (performanceMonitor == null)
                throw new ArgumentNullException(nameof(performanceMonitor));

            // Create ViewModel and set as DataContext
            _viewModel = new JarWidgetViewModel(performanceMonitor);
            DataContext = _viewModel;

            // Set up click-through behavior (except for drag handle)
            // Uses Win32 API via WindowUtils
            SetClickThrough();

            // Set window properties
            Topmost = true;
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
    }
}
