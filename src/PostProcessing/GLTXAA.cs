using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    //Incomplete
    internal sealed unsafe class GLTXAA : Effect, IDisposable
    {
        public GLTXAA(int width, int height, int threshold = 70)
        {
            previousFrame = new PixelBuffer(width, height);
            temporaryBuffer = new PixelBuffer(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private PixelBuffer previousFrame;
        private PixelBuffer temporaryBuffer;
        private int sqrThreshold;

        public bool EdgeDettection { get; set; } = false;

        public override void Process(PixelBuffer target)
        {
            if (target.width != previousFrame.width || target.height != previousFrame.height)
                return;

            temporaryBuffer.FastClone(target);

            if (!EdgeDettection)
            {
                Parallel.For(1, target.height - 1, (y) =>
                {
                    for (int x = 1; x < target.width - 1; x++)
                    {
                        int cur = target.width * y + x;
                        int up = target.width * (y - 1) + x;
                        int left = target.width * y + x - 1;
                        int down = target.width * (y + 1) + x;
                        int right = target.width * y + x + 1;

                        float differenceV = dist(
                            target.uint0[up],
                            target.uint0[down]);

                        float differenceH = dist(
                            target.uint0[right],
                            target.uint0[left]);

                        float factor = differenceH > differenceV ? differenceH : differenceV;
                        factor = 0.95f * adjust(factor);

                            temporaryBuffer.uint0[cur] = avg(
                                previousFrame.uint0[cur],
                                target.uint0[cur],
                                factor / 2);

                        //copy.buffer[cur] = (uint)(factor * 255) * 0x00010101 + 0xff000000;
                    }
                });

                target.FastClone(temporaryBuffer);
                previousFrame.FastClone(target);
            }
            else
            {
                Parallel.For(1, target.height - 1, (y) =>
                {
                    for (int x = 1; x < target.width - 1; x++)
                    {
                        int cur = target.width * y + x;
                        int up = target.width * (y - 1) + x;
                        int left = target.width * y + x - 1;
                        int down = target.width * (y + 1) + x;
                        int right = target.width * y + x + 1;

                        float differenceV = dist(
                            target.uint0[up],
                            target.uint0[down]);

                        float differenceH = dist(
                            target.uint0[right],
                            target.uint0[left]);

                        float factor = differenceH > differenceV ? differenceH : differenceV;
                        factor = 0.95f * adjust(factor);

                        temporaryBuffer.uint0[cur] = (uint)(factor * 255) * 0x10101u;
                    }
                });
                previousFrame.FastClone(target);
                target.FastClone(temporaryBuffer);
            }

            target.FastClone(temporaryBuffer);
            previousFrame.FastClone(target);
            return;

            float adjust(float x) => -x * x + 2 * x;

            float dist(uint pixel1, uint pixel2)
            {
                float sum = 0f;
                int tmp;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp * tmp / 255f;

                pixel1 >>= 8;
                pixel2 >>= 8;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp * tmp / 255f;

                pixel1 >>= 8;
                pixel2 >>= 8;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp * tmp / 255f;

                return sum / (3f * 255f);
            }

            uint avg(uint pixel1, uint pixel2, float factor1)
            {
                uint res = 0;
                for (int i = 0; i < 3; i++)
                {
                    res += (uint)(factor1 * (byte)pixel1 + (1 - factor1) * (byte)pixel2) << (8 * i);
                    pixel1 >>= 8;
                    pixel2 >>= 8;
                }
                res += 0xff000000;
                return res;
            }
        }

        public void Dispose()
        {
            previousFrame.Dispose();
        }
    }
}
