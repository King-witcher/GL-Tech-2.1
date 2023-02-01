using System;
using System.Threading.Tasks;

namespace Engine.Imaging.Processing
{
    public sealed unsafe class FFXAA : Effect, IDisposable
    {
        public bool ShowEdges { get; set; } = false;

        public FFXAA(int width, int height, int threshold = 128)
        {
            tempbuffer = new Image(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private Image tempbuffer;
        private int sqrThreshold;

        public override void Process(Image target)
        {
            if (target.Width != tempbuffer.Width || target.Height != tempbuffer.Height)
                return;

            if (!ShowEdges)
            {
                Image.BufferCopy(target, tempbuffer);
                Parallel.For(1, target.Height, i =>
                {
                    for (int j = 1; j < target.Width; j++)
                    {
                        int cur = target.Width * i + j;
                        int up = target.Width * (i - 1) + j;
                        int left = target.Width * i + j - 1;

                        int differenceV = dist(
                            ((uint*)target.Buffer)[cur],
                            ((uint*)target.Buffer)[up]);

                        int differenceH = dist(
                            ((uint*)target.Buffer)[cur],
                            ((uint*)target.Buffer)[left]);

                        if (differenceV >= sqrThreshold)
                            ((uint*)tempbuffer.Buffer)[target.Width * i + j] = avg(((uint*)target.Buffer)[up], ((uint*)target.Buffer)[cur]);
                        else if (differenceH >= sqrThreshold)
                            ((uint*)tempbuffer.Buffer)[target.Width * i + j] = avg(((uint*)target.Buffer)[left], ((uint*)target.Buffer)[cur]);
                    }
                });
            }
            else
            {
                Parallel.For(1, target.Height, i =>
                {
                    for (int j = 1; j < target.Width; j++)
                    {
                        int cur = target.Width * i + j;
                        int up = target.Width * (i - 1) + j;
                        int left = target.Width * i + j - 1;

                        int differenceV = dist(
                            ((uint*)target.Buffer)[cur],
                            ((uint*)target.Buffer)[up]);

                        int differenceH = dist(
                            ((uint*)target.Buffer)[cur],
                            ((uint*)target.Buffer)[left]);

                        if (differenceV >= sqrThreshold)
                            ((uint*)tempbuffer.Buffer)[target.Width * i + j] = 0xff0000;
                        else if (differenceH >= sqrThreshold)
                            ((uint*)tempbuffer.Buffer)[target.Width * i + j] = 0x0000ff;
                        else
                            ((uint*)tempbuffer.Buffer)[target.Width * i + j] = 0;
                    }
                });
            }

            Image.BufferCopy(tempbuffer, target);
            return;


            int dist(uint pixel1, uint pixel2)
            {
                int sum = 0;
                int tmp;

                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;
                pixel1 >>= 8;
                pixel2 >>= 8;
                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;
                pixel1 >>= 8;
                pixel2 >>= 8;
                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;

                return sum;
            }

            uint avg(uint pixel1, uint pixel2)
            {
                // B
                uint result = ((pixel1 & 0xffu) + (pixel2 & 0xffu)) / 2u;

                // G
                result |= ((pixel1 & 0xff00u) + (pixel2 & 0xff00u)) / 2u & 0xff00u;

                // R
                result |= ((pixel1 & 0xff0000u) + (pixel2 & 0xff0000u)) / 2u & 0xff0000u;

                return result;
            }
        }

        public void Dispose()
        {
            tempbuffer.Dispose();
        }
    }
}
