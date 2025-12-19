namespace GLTech.Processing
{
    //Incomplete
    internal sealed unsafe class GLTXAA : Effect, IDisposable
    {
        public GLTXAA(int width, int height, int threshold = 70)
        {
            previousFrame = new Image(width, height);
            temporaryBuffer = new Image(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private Image previousFrame;
        private Image temporaryBuffer;
        private int sqrThreshold;

        public bool EdgeDettection { get; set; } = false;

        public override void Process(Image target)
        {
            if (target.Width != previousFrame.Width || target.Height != previousFrame.Height)
                return;

            Image.BufferCopy(temporaryBuffer, target);

            if (!EdgeDettection)
            {
                Parallel.For(1, target.Height - 1, y =>
                {
                    for (int x = 1; x < target.Width - 1; x++)
                    {
                        int cur = target.Width * y + x;
                        int up = target.Width * (y - 1) + x;
                        int left = target.Width * y + x - 1;
                        int down = target.Width * (y + 1) + x;
                        int right = target.Width * y + x + 1;

                        float differenceV = dist(
                            ((uint*)target.Buffer)[up],
                            ((uint*)target.Buffer)[down]);

                        float differenceH = dist(
                            ((uint*)target.Buffer)[right],
                            ((uint*)target.Buffer)[left]);

                        float factor = differenceH > differenceV ? differenceH : differenceV;
                        factor = 0.95f * adjust(factor);

                        ((uint*)temporaryBuffer.Buffer)[cur] = avg(
                            ((uint*)previousFrame.Buffer)[cur],
                            ((uint*)target.Buffer)[cur],
                            factor / 2);

                        //copy.buffer[cur] = (uint)(factor * 255) * 0x00010101 + 0xff000000;
                    }
                });

                Image.BufferCopy(temporaryBuffer, target);
                Image.BufferCopy(target, previousFrame);
            }
            else
            {
                Parallel.For(1, target.Height - 1, y =>
                {
                    for (int x = 1; x < target.Width - 1; x++)
                    {
                        int cur = target.Width * y + x;
                        int up = target.Width * (y - 1) + x;
                        int left = target.Width * y + x - 1;
                        int down = target.Width * (y + 1) + x;
                        int right = target.Width * y + x + 1;

                        float differenceV = dist(
                            ((uint*)target.Buffer)[up],
                            ((uint*)target.Buffer)[down]);

                        float differenceH = dist(
                            ((uint*)target.Buffer)[right],
                            ((uint*)target.Buffer)[left]);

                        float factor = differenceH > differenceV ? differenceH : differenceV;
                        factor = 0.95f * adjust(factor);

                        ((uint*)temporaryBuffer.Buffer)[cur] = (uint)(factor * 255) * 0x10101u;
                    }
                });
                Image.BufferCopy(target, previousFrame);
                Image.BufferCopy(temporaryBuffer, target);
            }

            Image.BufferCopy(temporaryBuffer, target);
            Image.BufferCopy(target, previousFrame);
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
