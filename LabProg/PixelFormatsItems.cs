using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg
{
    class PixelFormatsItem
    {
        public int Value { get; set; }
        public string Description { get; set; }
    }
    
    static class PixelFormatsItems
    {

        public static  List<PixelFormatsItem> GetList()
        {
            return new List<PixelFormatsItem> {
                new PixelFormatsItem{Description= "Undefined", Value=0 },
                new PixelFormatsItem{Description= "DontCare", Value=0 },
                new PixelFormatsItem{Description= "Max", Value=15 },
                new PixelFormatsItem{Description= "Indexed", Value=65536 },
                new PixelFormatsItem{Description= "Gdi", Value=131072 },
                new PixelFormatsItem{Description= "Format16bppRgb555", Value=135173 },
                new PixelFormatsItem{Description= "Format16bppRgb565", Value=135174 },
                new PixelFormatsItem{Description= "Format24bppRgb", Value=137224 },
                new PixelFormatsItem{Description= "Format32bppRgb", Value=139273 },
                new PixelFormatsItem{Description= "Format1bppIndexed", Value=196865 },
                new PixelFormatsItem{Description= "Format4bppIndexed", Value=197634 },
                new PixelFormatsItem{Description= "Format8bppIndexed", Value=198659 },
                new PixelFormatsItem{Description= "Alpha", Value=262144 },
                new PixelFormatsItem{Description= "Format16bppArgb1555", Value=397319 },
                new PixelFormatsItem{Description= "PAlpha", Value=524288 },
                new PixelFormatsItem{Description= "Format32bppPArgb", Value=925707 },
                new PixelFormatsItem{Description= "Extended", Value=1048576 },
                new PixelFormatsItem{Description= "Format16bppGrayScale", Value=1052676 },
                new PixelFormatsItem{Description= "Format48bppRgb", Value=1060876 },
                new PixelFormatsItem{Description= "Format64bppPArgb", Value=1851406 },
                new PixelFormatsItem{Description= "Canonical", Value=2097152 },
                new PixelFormatsItem{Description= "Format32bppArgb", Value=2498570 },
                new PixelFormatsItem{Description= "Format64bppArgb", Value=3424269 },
            };
        }
    }
}
