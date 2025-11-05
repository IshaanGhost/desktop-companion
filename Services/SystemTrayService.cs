using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesktopCompanions.Services
{
    /// <summary>
    /// Service for managing system tray icon and menu
    /// </summary>
    public class SystemTrayService : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private bool _disposed = false;

        public event EventHandler? ShowConfiguration;
        public event EventHandler? ExitApplication;

        public void Initialize()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                Text = "Desktop Companions",
                Visible = true
            };

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Settings", null, OnShowConfiguration);
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitApplication);

            _notifyIcon.DoubleClick += OnShowConfiguration;
        }

        private void OnShowConfiguration(object? sender, EventArgs e)
        {
            ShowConfiguration?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitApplication(object? sender, EventArgs e)
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        private Icon CreateTrayIcon()
        {
            // Create a 16x16 icon showing both cat and jar
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);

                // Draw jar on the left (8x12)
                var jarBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
                var jarPen = new Pen(Color.LightBlue, 1);
                graphics.FillRectangle(jarBrush, 1, 2, 6, 10);
                graphics.DrawRectangle(jarPen, 1, 2, 6, 10);
                
                // Draw goblin circle inside jar
                var goblinBrush = new SolidBrush(Color.Orange);
                graphics.FillEllipse(goblinBrush, 2, 5, 4, 4);

                // Draw cat on the right (simple cat shape)
                var catBrush = new SolidBrush(Color.Gray);
                var catPen = new Pen(Color.Black, 1);
                // Cat head
                graphics.FillEllipse(catBrush, 10, 3, 5, 5);
                graphics.DrawEllipse(catPen, 10, 3, 5, 5);
                // Cat body
                graphics.FillEllipse(catBrush, 11, 8, 4, 6);
                graphics.DrawEllipse(catPen, 11, 8, 4, 6);

                // Convert to icon
                var hIcon = bitmap.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _notifyIcon?.Dispose();
            _disposed = true;
        }
    }
}
