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
            Bitmap resizedMap = ImageManipulation.ResizeImage(newImage, 4, 1);
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
                _linkGrid[0] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
            }
        }

        /// <summary>
        /// From a given resized screenshot, an ambilight effect will be created for the keyboard
        /// </summary>
        /// <param name="map">resized screenshot</param>
        private void ApplyImageToGrid(Bitmap map)
        {
            using (var fastBitmap = new FastBitmap(map))
            {
                fastBitmap.Lock();

                //Iterating over each key and set it to the corrosponding color of the resized Screenshot
                for (int i = 1; i < Colore.Effects.ChromaLink.ChromaLinkConstants.MaxLeds; i++)
                {
                    Color color = fastBitmap.GetPixel(i - 1, 0);
                    _linkGrid[i] = new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
                }
            }
        }
    }
}
