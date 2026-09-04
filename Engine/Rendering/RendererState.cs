namespace Engine.Rendering
{
    // TODO: Stop allocating this on the heap
    // Use a RenderJob instead
    internal unsafe readonly struct RendererState
    {
        // Height that a column at 1 unit of distance from the spectator should be drawn on the screen.
        public readonly float colHeight1;
        public readonly float step0;
        public readonly int width;
        public readonly int height;

        internal RendererState(int width, int height, float hfov_deg)
        {
            const float DEG_TO_RAD = MathF.PI / 180f;
            var tan = MathF.Tan(DEG_TO_RAD * hfov_deg * .5f);
            step0 = 2f * tan / width;
            colHeight1 = width / (2f * tan);
            this.width = width;
            this.height = height;
        }

        public static RendererState* Create(int width, int height, float hfov)
        {
            RendererState* result = Memory.Alloc<RendererState>(alignment: 64);
            *result = new(width, height, hfov);
            return result;
        }

        public static void Destroy(RendererState* cache)
        {
            Memory.Free(cache);
        }
    }
}
