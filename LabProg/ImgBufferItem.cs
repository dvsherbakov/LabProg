using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace LabProg
{
    class ImgBufferItem
    {
        public int[] Buffer { get; set; }
        public Stopwatch Mark { get; set; }
        private bool IsNormalized;
        private readonly int XResolution;
        private readonly int YResolution;

        public ImgBufferItem(int[] buff)
        {
            Buffer = buff;
            Mark = Stopwatch.StartNew();
            IsNormalized = false;
            XResolution = 1024;
            YResolution = 1024;
        }

        private void NormalizeBuffer()
        {
            if (IsNormalized) return;
            var min = Buffer.Min();
            var max = Buffer.Max();
            var cScale = (256.0f / (max - min));
            for (var i = 0; i < Buffer.Length; i++)
                Buffer[i] = (byte)((Buffer[i] - min) * cScale);
            IsNormalized = true;
        }

        private void SaveToBitmap()
        {
            Bitmap Bm = new Bitmap(XResolution, YResolution, PixelFormat.Format24bppRgb);
            var b = new Bitmap(XResolution, YResolution, PixelFormat.Format8bppIndexed);
            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;
            for (int y = 0; y < YResolution; y++)
            {
                for (int x = 0; x < XResolution; x++)
                {
                    int Value = Buffer[x + (y * XResolution)];
                    Color C = ncp.Entries[Value];
                    Bm.SetPixel(x, y, C);
                }
            }
            var fName = DateTime.Now.ToString().Replace(":", "-").Replace(".", "_") +
                Mark.Elapsed.ToString().Replace(":", "-").Replace(".", "_") + ".bmp";
            Bm.Save(fName);
            Mark.Stop();
        }

        public void Save()
        {
            NormalizeBuffer();
            SaveToBitmap();
        }
    }
}
