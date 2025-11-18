using System;
using System.Drawing;
using Ambilight.GUI;
using Ambilight.Util;
using Colore;
using Colore.Effects.Headset;
using Microsoft.VisualBasic;
using ColoreColor = Colore.Data.Color;

namespace Ambilight.Logic
{
    public class HeadsetLogic : IDeviceLogic
    {
        private TraySettings _settings;
        private IChroma _chroma;
        private CustomHeadsetEffect _headsetGrid = CustomHeadsetEffect.Create();

        public HeadsetLogic(TraySettings settings, IChroma chroma)
        {
            _settings = settings;
            _chroma = chroma;
        }

        public void Process(Bitmap newImage)
        {
            Bitmap resizedMap = ImageManipulation.ResizeImage(newImage, 2, 1);
            Bitmap saturatedMap = ImageManipulation.ApplySaturation(resizedMap, _settings.Saturation);
            resizedMap.Dispose(); // Dispose the intermediate bitmap

            ApplyPictureToGrid(saturatedMap);
            _chroma.Headset.SetCustomAsync(_headsetGrid);
            saturatedMap.Dispose();
        }

        private void ApplyPictureToGrid(Bitmap map)
        {
            using (var fastBitmap = new FastBitmap(map))
            {
                fastBitmap.Lock();
                _headsetGrid[0] = toColoreColor(fastBitmap.GetPixel(0, 0));
                _headsetGrid[1] = toColoreColor(fastBitmap.GetPixel(1, 0));
            }
        }

        private ColoreColor toColoreColor(Color color)
        {
            return new ColoreColor((byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}