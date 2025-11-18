using System.Drawing;
using Ambilight.GUI;
using Ambilight.Util;
using Colore;
using Colore.Effects.Mouse;
using ColoreColor = Colore.Data.Color;

namespace Ambilight.Logic
{

    /// <summary>
    /// Handles the Ambilight Effect for the mouse
    /// </summary>
    class MouseLogic : IDeviceLogic
    {
        private TraySettings _settings;
        private IChroma _chroma;
        private CustomMouseEffect _mouseGrid = CustomMouseEffect.Create();

        public MouseLogic(TraySettings settings, IChroma chromaInstance)
        {
            this._settings = settings;
            this._chroma = chromaInstance;
        }

        /// <summary>
        /// Processes a ScreenShot and creates an Ambilight Effect for the mouse
        /// </summary>
        /// <param name="newImage">ScreenShot</param>
        public void Process(Bitmap newImage)
        {
            Bitmap resizedMap = ImageManipulation.ResizeImage(newImage, MouseConstants.MaxColumns,
                    MouseConstants.MaxRows);
            Bitmap saturatedMap = ImageManipulation.ApplySaturation(resizedMap, _settings.Saturation);
            resizedMap.Dispose(); // Dispose the intermediate bitmap

            ApplyPictureToGrid(saturatedMap);
            _chroma.Mouse.SetGridAsync(_mouseGrid);
            saturatedMap.Dispose();
        }

        /// <summary>
        /// From a given resized screenshot, an ambilight effect will be created for the mouse
        /// </summary>
        /// <param name="mapMousePad">resized screenshot</param>
        /// <param name="mousePadGrid">effect grid</param>
        /// <returns>EffectGrid</returns>
        private void ApplyPictureToGrid(Bitmap mapMouse)
        {
            using (var fastBitmap = new FastBitmap(mapMouse))
            {
                fastBitmap.Lock();

                for (var r = 0; r < MouseConstants.MaxRows; r++)
                {
                    for (var c = 0; c < MouseConstants.MaxColumns; c++)
                    {
                        Color color;

                        if (_settings.AmbiModeEnabled)
                            color = fastBitmap.GetPixel(DeviceConstants.Mouse.AmbiModeSampleX, DeviceConstants.Mouse.AmbiModeSampleY);
                        else
                            color = fastBitmap.GetPixel(c, r);


                        _mouseGrid[r, c] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }
                }
            }
        }
    }


}
