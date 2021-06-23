using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    public sealed class GrayScale : Effect
    {
        public override void Process(PixelBuffer target)
        {
            target.Foreach(RGBToGray);
        }

        private RGB RGBToGray(RGB original) => original.Brightness * 0x010101u;
    }
}
