

namespace GLTech.Imaging.Processing
{
    public sealed class GrayScale : Effect
    {
        public override void Process(Image target) =>
            target.Foreach(RGBToGray);

        private Color RGBToGray(Color original) =>
           original.Brightness * 0x010101u;
    }
}
