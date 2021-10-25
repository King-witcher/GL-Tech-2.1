

namespace GLTech2.Drawing.StandardEffects
{
    public sealed class GrayScale : PostProcessing
    {
        public override void Process(PixelBuffer target) =>
            target.Foreach(RGBToGray);

        private RGB RGBToGray(RGB original) =>
           original.Brightness * 0x010101u;
    }
}
