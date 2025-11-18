using System.Drawing;
using Ambilight.GUI;
using Ambilight.Util;
using Colore;
using Colore.Effects.Mousepad;
using ColoreColor = Colore.Data.Color;

namespace Ambilight.Logic
{
    /// <summary>
    /// Handles the Ambilight Effect for the mousepad
    /// </summary>
    class MousePadLogic : IDeviceLogic
    {
        private TraySettings _settings;
        private IChroma _chroma;
        private CustomMousepadEffect _mousepadGrid = CustomMousepadEffect.Create();

        public MousePadLogic(TraySettings settings, IChroma chromaInstance)
        {
            this._settings = settings;
            this._chroma = chromaInstance;
        }


        /// <summary>
        /// Processes a ScreenShot and creates an Ambilight Effect for the mousepad
        /// </summary>
        /// <param name="newImage">ScreenShot</param>
        public void Process(Bitmap newImage)
        {
            Bitmap resizedMap = ImageManipulation.ResizeImage(newImage, DeviceConstants.Mousepad.GridWidth, DeviceConstants.Mousepad.GridHeight);
            Bitmap saturatedMap = ImageManipulation.ApplySaturation(resizedMap, _settings.Saturation);
            resizedMap.Dispose(); // Dispose the intermediate bitmap

            ApplyPictureToGrid(saturatedMap);
            _chroma.Mousepad.SetCustomAsync(_mousepadGrid);
            saturatedMap.Dispose();
        }

        /// <summary>
        /// From a given resized screenshot, an ambilight effect will be created for the mousepad
        ///^->>i
        ///^   V
        ///^   V
        ///<<<<V
        ///
        /// </summary>
        /// <param name="mapMousePad">resized screenshot</param>
        /// <returns></returns>
        private void ApplyPictureToGrid(Bitmap mapMousePad)
        {
            using (var fastBitmap = new FastBitmap(mapMousePad))
            {
                fastBitmap.Lock();

                if (!_settings.AmbiModeEnabled)
                {
                    // Right edge LEDs (0-3)
                    for (int i = 0; i <= DeviceConstants.Mousepad.RightEdgeMaxIndex; i++)
                    {
                        Color color = fastBitmap.GetPixel(DeviceConstants.Mousepad.GridWidth - 1, i);
                        _mousepadGrid[i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }

                    // Bottom-right corner LED (4)
                    Color colorC = fastBitmap.GetPixel(DeviceConstants.Mousepad.GridWidth - 1, DeviceConstants.Mousepad.BottomRightCornerIndex);
                    _mousepadGrid[DeviceConstants.Mousepad.BottomRightCornerIndex] = new ColoreColor((byte)colorC.R, (byte)colorC.G, (byte)colorC.B);

                    // Bottom edge LEDs (5-10)
                    for (int i = DeviceConstants.Mousepad.BottomEdgeStartX; i >= DeviceConstants.Mousepad.BottomEdgeEndX; i--)
                    {
                        Color color = fastBitmap.GetPixel(i, DeviceConstants.Mousepad.GridHeight - 1);
                        _mousepadGrid[10 - i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }

                    // Left edge + corner LEDs (11-14)
                    for (int i = DeviceConstants.Mousepad.LeftEdgeMaxIndex; i >= 0; i--)
                    {
                        Color color = fastBitmap.GetPixel(0, i);
                        _mousepadGrid[14 - i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }
                }
                else
                {
                    // Ambi mode: use solid colors from specific regions

                    // Right edge zone
                    for (int i = 0; i <= DeviceConstants.Mousepad.RightEdgeMaxIndex; i++)
                    {
                        Color color = fastBitmap.GetPixel(DeviceConstants.Mousepad.AmbiRightX, DeviceConstants.Mousepad.AmbiRightY);
                        _mousepadGrid[i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }

                    // Bottom-right corner
                    Color colorC = fastBitmap.GetPixel(DeviceConstants.Mousepad.AmbiRightX, DeviceConstants.Mousepad.AmbiRightY);
                    _mousepadGrid[DeviceConstants.Mousepad.BottomRightCornerIndex] = new ColoreColor((byte)colorC.R, (byte)colorC.G, (byte)colorC.B);

                    // Bottom edge zone
                    for (int i = DeviceConstants.Mousepad.BottomEdgeStartX; i >= DeviceConstants.Mousepad.BottomEdgeEndX; i--)
                    {
                        Color color = fastBitmap.GetPixel(DeviceConstants.Mousepad.AmbiBottomX, DeviceConstants.Mousepad.AmbiBottomY);
                        _mousepadGrid[10 - i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }

                    // Left edge zone
                    for (int i = DeviceConstants.Mousepad.LeftEdgeMaxIndex; i >= 0; i--)
                    {
                        Color color = fastBitmap.GetPixel(DeviceConstants.Mousepad.AmbiLeftX, DeviceConstants.Mousepad.AmbiLeftY);
                        _mousepadGrid[14 - i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                    }
                }
            }
        }
    }
}
