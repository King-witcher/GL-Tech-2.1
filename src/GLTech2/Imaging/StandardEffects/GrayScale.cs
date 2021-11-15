

namespace GLTech2.Imaging.StandardEffects
{
    public sealed class GrayScale : ImageProcessing
    {
        public override void Process(ImageData target) =>
            target.Foreach(RGBToGray);

        private Pixel RGBToGray(Pixel original) =>
           original.Brightness * 0x010101u;
    }
}
