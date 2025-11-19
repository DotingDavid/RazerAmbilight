using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Ambilight.Util
{
    /// <summary>
    /// Fast bitmap accessor using LockBits for efficient pixel access
    /// This is 100-1000x faster than using GetPixel()
    /// </summary>
    public class FastBitmap : IDisposable
    {
        private readonly Bitmap _bitmap;
        private BitmapData _bitmapData;
        private IntPtr _ptr;
        private int _stride;
        private bool _locked;

        public int Width { get; }
        public int Height { get; }

        public FastBitmap(Bitmap bitmap)
        {
            _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        /// <summary>
        /// Locks the bitmap for reading. Must be called before GetPixel()
        /// </summary>
        public void Lock()
        {
            if (_locked) return;

            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            _bitmapData = _bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            _ptr = _bitmapData.Scan0;
            _stride = _bitmapData.Stride;
            _locked = true;
        }

        /// <summary>
        /// Unlocks the bitmap. Should be called when done reading pixels
        /// </summary>
        public void Unlock()
        {
            if (!_locked) return;

            _bitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _locked = false;
        }

        /// <summary>
        /// Gets a pixel color at the specified coordinates
        /// Lock() must be called first
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            if (!_locked)
                throw new InvalidOperationException("Bitmap must be locked before calling GetPixel()");

            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds");

            unsafe
            {
                byte* row = (byte*)_ptr + (y * _stride);
                int offset = x * 4; // 4 bytes per pixel (BGRA)

                byte b = row[offset];
                byte g = row[offset + 1];
                byte r = row[offset + 2];
                byte a = row[offset + 3];

                return Color.FromArgb(a, r, g, b);
            }
        }

        public void Dispose()
        {
            Unlock();
        }
    }
}
