using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    /// <summary>
    /// A faster version of FXAA that only blurs each edge.
    /// </summary>
    public sealed unsafe class FFXAA : Effect, IDisposable
    {
        /// <summary>
        /// Shows every dettected edge.
        /// </summary>
        public bool ShowEdges { get; set; } = false;

        /// <summary>
        /// Gets a new instance of FFXAA respecting the screen dimentions.
        /// </summary>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        /// <param name="threshold">The threshold that determines whether something is or not a border</param>
        public FFXAA(int width, int height, int threshold = 128)
        {
            temporaryBuffer = new PixelBuffer(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private PixelBuffer temporaryBuffer;
        private int sqrThreshold;

        public override void Process(PixelBuffer target)
        {
            if (target.width != temporaryBuffer.width || target.height != temporaryBuffer.height)
                return;

            if (!ShowEdges)
            {
                temporaryBuffer.Clone(target);
                Parallel.For(1, target.height, (i) =>
                {
                    for (int j = 1; j < target.width; j++)
                    {
                        int cur = target.width * i + j;
                        int up = target.width * (i - 1) + j;
                        int left = target.width * i + j - 1;

                        int differenceV = dist(
                            target.uint0[cur],
                            target.uint0[up]);

                        int differenceH = dist(
                            target.uint0[cur],
                            target.uint0[left]);

                        if (differenceV >= sqrThreshold)
                            temporaryBuffer.uint0[target.width * i + j] = avg(target.uint0[up], target.uint0[cur]);
                        else if (differenceH >= sqrThreshold)
                            temporaryBuffer.uint0[target.width * i + j] = avg(target.uint0[left], target.uint0[cur]);
                    }
                });
            }
            else
            {
                Parallel.For(1, target.height, (i) =>
                {
                    for (int j = 1; j < target.width; j++)
                    {
                        int cur = target.width * i + j;
                        int up = target.width * (i - 1) + j;
                        int left = target.width * i + j - 1;

                        int differenceV = dist(
                            target.uint0[cur],
                            target.uint0[up]);

                        int differenceH = dist(
                            target.uint0[cur],
                            target.uint0[left]);

                        if (differenceV >= sqrThreshold)
                            temporaryBuffer.uint0[target.width * i + j] = 0xff0000;
                        else if (differenceH >= sqrThreshold)
                            temporaryBuffer.uint0[target.width * i + j] = 0x0000ff;
                        else
                            temporaryBuffer.uint0[target.width * i + j] = 0;
                    }
                });
            }

            target.Clone(temporaryBuffer);
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
            temporaryBuffer.Dispose();
        }
    }
}
