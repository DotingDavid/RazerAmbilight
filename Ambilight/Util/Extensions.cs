using System.Drawing;

namespace Ambilight.Util
{
    public static class Extensions
    {
        /// <summary>
        /// Crops a bitmap to the specified rectangle.
        /// WARNING: This method DISPOSES the input bitmap to prevent memory leaks.
        /// Do not use the input bitmap after calling this method.
        /// </summary>
        /// <param name="bitmap">The bitmap to crop (will be disposed)</param>
        /// <param name="rectangle">The rectangle to crop to</param>
        /// <returns>A new cropped bitmap</returns>
        public static Bitmap CropAtRectangle(this Bitmap bitmap, Rectangle rectangle)
        {
            Bitmap newBitmap = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(bitmap, -rectangle.X, -rectangle.Y);
                bitmap.Dispose(); // Dispose input to prevent memory leak
                newBitmap.SetResolution(rectangle.Width, rectangle.Height);
                return newBitmap;
            }
        }
    }
}