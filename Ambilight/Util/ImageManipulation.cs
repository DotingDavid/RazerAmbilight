using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Ambilight.Util;
using NLog;

namespace Ambilight
{
    class ImageManipulation
    {

        private static Logger _log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Resize an image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="cropSides">If true, crops a 21:9 image to 16:9 before resizing</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height, bool cropSides = false)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");

            try
            {
                if (cropSides)
                {
                    // Validate image dimensions are suitable for 21:9 crop
                    if (image.Width < 21 || image.Height < 1)
                    {
                        _log.Warn($"Image too small for ultrawide crop: {image.Width}x{image.Height}. Using normal resize.");
                        return new Bitmap(image, width, height);
                    }

                    // Calculate crop rectangle for 21:9 to 16:9 conversion
                    int cropX = Convert.ToInt32((image.Width / 21.0) * 2.5);
                    int cropWidth = Convert.ToInt32((image.Width / 21.0) * 16);

                    // Validate calculated crop rectangle
                    if (cropX < 0 || cropWidth <= 0 || cropX + cropWidth > image.Width)
                    {
                        _log.Warn($"Invalid crop rectangle calculated: x={cropX}, width={cropWidth}. Using normal resize.");
                        return new Bitmap(image, width, height);
                    }

                    // Cuts down a 21:9 image to a 16:9 image by removing the outer sides
                    using (var croppedImage = new Bitmap(image).CropAtRectangle(new Rectangle(cropX, 0, cropWidth, image.Height)))
                    {
                        return new Bitmap(croppedImage, width, height);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error while resizing the image. width: {width} height: {height} cropSides: {cropSides}. Falling back to normal resize.");
            }

            return new Bitmap(image, width, height);
        }

        /// <summary>
        /// Applies a given saturation value to a Bitmap.
        /// </summary>
        /// <param name="srcBitmap">Bitmap</param>
        /// <param name="saturation">Saturation Value</param>
        /// <returns></returns>
        public static Bitmap ApplySaturation(Bitmap srcBitmap, float saturation)
        {
            float rWeight = 0.3086f;
            float gWeight = 0.6094f;
            float bWeight = 0.0820f;

            float a = (1.0f - saturation) * rWeight + saturation;
            float b = (1.0f - saturation) * rWeight;
            float c = (1.0f - saturation) * rWeight;
            float d = (1.0f - saturation) * gWeight;
            float e = (1.0f - saturation) * gWeight + saturation;
            float f = (1.0f - saturation) * gWeight;
            float g = (1.0f - saturation) * bWeight;
            float h = (1.0f - saturation) * bWeight;
            float i = (1.0f - saturation) * bWeight + saturation;

            Bitmap returnBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height);

            // Create a Graphics
            using (Graphics gr = Graphics.FromImage(returnBitmap))
            {
                // ColorMatrix elements
                float[][] ptsArray = {
                    new float[] {a,  b,  c,  0, 0},
                    new float[] {d,  e,  f,  0, 0},
                    new float[] {g,  h,  i,  0, 0},
                    new float[] {0,  0,  0,  1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };
                // Create ColorMatrix
                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                // Create ImageAttributes
                ImageAttributes imgAttribs = new ImageAttributes();
                // Set color matrix
                imgAttribs.SetColorMatrix(clrMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Default);
                // Draw Image with no effects
                //gr.DrawImage(srcBitmap, 0, 0, 200, 200);
                // Draw Image with image attributes
                gr.DrawImage(srcBitmap,
                    new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height),
                    0, 0, srcBitmap.Width, srcBitmap.Height,
                    GraphicsUnit.Pixel, imgAttribs);
            }

            return returnBitmap;
        }
    }
}