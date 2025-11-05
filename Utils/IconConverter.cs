using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace DesktopCompanions.Utils
{
    /// <summary>
    /// Utility class for converting System.Drawing.Icon to WPF ImageSource
    /// </summary>
    public static class IconConverter
    {
        public static BitmapImage IconToBitmapImage(Icon icon, int size = 32)
        {
            if (icon == null) return null!;

            using (var bitmap = new Bitmap(size, size))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawIcon(icon, new Rectangle(0, 0, size, size));

                var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            if (bitmap == null) return null!;

            var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}

