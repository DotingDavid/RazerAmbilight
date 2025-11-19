using System.Drawing;
using Ambilight.GUI;
using Ambilight.Util;
using Colore;
using Colore.Effects.ChromaLink;
using ColoreColor = Colore.Data.Color;


namespace Ambilight.Logic
{

    /// <summary>
    /// Handles the Ambilight Effect for the Link connection
    /// </summary>
    class LinkLogic : IDeviceLogic
    {
        private GUI.TraySettings _settings;
        private CustomChromaLinkEffect _linkGrid = CustomChromaLinkEffect.Create();
        private IChroma _chroma;
        
        public LinkLogic(TraySettings settings, IChroma chromaInstance)
        {
            this._settings = settings;
            this._chroma = chromaInstance;
        }

        /// <summary>
        /// Processes a ScreenShot and creates an Ambilight Effect for the chroma link
        /// </summary>
        /// <param name="newImage">ScreenShot</param>
        public void Process(Bitmap newImage)
        {
            Bitmap resizedMap = ImageManipulation.ResizeImage(newImage, DeviceConstants.ChromaLink.GridWidth, DeviceConstants.ChromaLink.GridHeight);
            Bitmap saturatedMap = ImageManipulation.ApplySaturation(resizedMap, _settings.Saturation);
            resizedMap.Dispose(); // Dispose the intermediate bitmap

            ApplyImageToGrid(saturatedMap);

            Bitmap singlePixel = ImageManipulation.ResizeImage(saturatedMap, 1, 1);
            ApplyC1(singlePixel);
            singlePixel.Dispose(); // Dispose the temporary bitmap

            _chroma.ChromaLink.SetCustomAsync(_linkGrid);
            saturatedMap.Dispose();
        }

        private void ApplyC1(Bitmap map)
        {
            using (var fastBitmap = new FastBitmap(map))
            {
                fastBitmap.Lock();
                Color color = fastBitmap.GetPixel(0, 0);
                _linkGrid[DeviceConstants.ChromaLink.FirstLedIndex] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
            }
        }

        /// <summary>
        /// From a given resized screenshot, an ambilight effect will be created for the Chroma Link
        /// </summary>
        /// <param name="map">resized screenshot</param>
        private void ApplyImageToGrid(Bitmap map)
        {
            using (var fastBitmap = new FastBitmap(map))
            {
                fastBitmap.Lock();

                // Map grid pixels to Chroma Link LEDs (starting from LED 1)
                for (int i = DeviceConstants.ChromaLink.StandardLedsStartIndex; i < Colore.Effects.ChromaLink.ChromaLinkConstants.MaxLeds; i++)
                {
                    Color color = fastBitmap.GetPixel(i - DeviceConstants.ChromaLink.StandardLedsStartIndex, 0);
                    _linkGrid[i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                }
            }
        }
    }
}
