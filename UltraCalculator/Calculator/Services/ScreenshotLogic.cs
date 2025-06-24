using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Calculator.Services
{
    public static class ScreenshotLogic
    {
        public static BitmapSource CaptureScreenArea(Rect rect)
        {
            //MessageBox.Show($"{rect.Width} vs {rect.Height}");
            if (rect.Width == 0 || rect.Height == 0) return null;
            using var screenBmp = new Bitmap((int)rect.Width, (int)rect.Height);
            using var g = Graphics.FromImage(screenBmp);
            g.CopyFromScreen((int)rect.X, (int)rect.Y, 0, 0, screenBmp.Size);
            return Imaging.CreateBitmapSourceFromHBitmap(
                screenBmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
        }

        public static Bitmap BitmapSourceToBitmap(BitmapSource source)
        {
            if (source == null || source.PixelWidth == 0 || source.PixelHeight == 0) return null;
            using var ms = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return new Bitmap(ms);
        }
    }
}
