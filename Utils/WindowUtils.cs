using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DesktopCompanion
{
    /// <summary>
    /// Utility class for window manipulation (click-through, etc.)
    /// </summary>
    public static class WindowUtils
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTTRANSPARENT = -1;
        private const int HTCLIENT = 1;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Makes the window click-through except for interactive elements
        /// Uses HitTest to allow clicks on specific elements while passing through others
        /// </summary>
        public static void SetClickThrough(Window window)
        {
            window.Background = Brushes.Transparent;
            
            // Override window procedure to handle hit testing
            window.SourceInitialized += (s, e) =>
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero) return;

                var source = HwndSource.FromHwnd(hwnd);
                source?.AddHook(WndProc);
            };
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                var hwndSource = HwndSource.FromHwnd(hwnd);
                
                if (hwndSource != null)
                {
                    // Convert screen coordinates to window coordinates
                var x = (short)(lParam.ToInt32() & 0xFFFF);
                var y = (short)(lParam.ToInt32() >> 16);
                
                // Get window position
                var window = hwndSource.RootVisual as Window;
                if (window != null)
                {
                    var screenPoint = new Point(x, y);
                    var windowPoint = window.PointFromScreen(screenPoint);
                    
                    // Perform hit test
                    var hitTestResult = VisualTreeHelper.HitTest(hwndSource.RootVisual, windowPoint);
                    
                    if (hitTestResult != null)
                    {
                        var element = hitTestResult.VisualHit;
                        // Check if we hit an interactive element (Border with drag handle)
                        while (element != null)
                        {
                            if (element is System.Windows.Controls.Border border && 
                                border.Name == "DragHandle")
                            {
                                // Allow interaction with drag handle
                                handled = false; // Let default handling occur
                                return IntPtr.Zero;
                            }
                            element = VisualTreeHelper.GetParent(element);
                        }
                    }
                }
                }
                
                // Transparent to clicks (click-through)
                handled = true;
                return new IntPtr(HTTRANSPARENT);
            }
            
            return IntPtr.Zero;
        }

        /// <summary>
        /// Enables full click-through for the window (use with caution)
        /// </summary>
        public static void EnableFullClickThrough(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Disables click-through for the window
        /// </summary>
        public static void DisableClickThrough(Window window)
        {
            // Ensure window handle is initialized
            if (!window.IsLoaded)
            {
                window.Loaded += (s, e) => DisableClickThrough(window);
                return;
            }

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                // If handle not ready, try again after SourceInitialized
                window.SourceInitialized += (s, e) => DisableClickThrough(window);
                return;
            }

            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }
    }
}
