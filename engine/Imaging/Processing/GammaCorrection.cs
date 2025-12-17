using System;
using System.Threading.Tasks;

namespace Engine.Imaging.Processing
{
    public unsafe sealed class GammaCorrection : Effect
    {
        float gamma;
        public float Gamma
        {
            get => gamma;
            set => gamma = value;
        }

        public GammaCorrection()
        {
            gamma = 2;
        }

        public GammaCorrection(float factor = 2f)
        {
            this.gamma = factor;
        }

        public override void Process(Image target)
        {
            Parallel.For(0, target.Width, x =>
            {
                for (int y = 0; y < target.Height; y++)
                {
                    int cur = target.Width * y + x;

                    Color color = ((Color*)target.Buffer)[cur];
                    ((Color*)target.Buffer)[cur] = (
                        (byte)(255f * Math.Pow(color.R / 255f, gamma)),
                        (byte)(255f * Math.Pow(color.G / 255f, gamma)),
                        (byte)(255f * Math.Pow(color.B / 255f, gamma)));
                }
            });
        }
    }
}
