using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
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

        public override void Process(PixelBuffer target)
        {
            Parallel.For(0, target.width, (x) =>
            {
                for (int y = 0; y < target.height; y++)
                {
                    int cur = target.width * y + x;

                    RGB color = target.uint0[cur];
                    target.RGB0[cur] = (
                        (byte)(255f * Math.Pow(color.R / 255f, gamma)),
                        (byte)(255f * Math.Pow(color.G / 255f, gamma)),
                        (byte)(255f * Math.Pow(color.B / 255f, gamma)));
                }
            });
        }
    }
}
