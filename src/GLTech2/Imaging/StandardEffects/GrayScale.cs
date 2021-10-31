

namespace GLTech2.Imaging.StandardEffects
{
    public sealed class GrayScale : ImageProcessing
    {
        public override void Process(PixelBuffer target) =>
            target.Foreach(RGBToGray);

        private Color RGBToGray(Color original) =>
           original.Brightness * 0x010101u;
    }
}
