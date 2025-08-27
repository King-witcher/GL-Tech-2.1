using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderCache : IDisposable
    {
        internal static int count;
        // Height that a column at 1 unit of distance from the spectator should be drawn on the screen.
        internal readonly float colHeight1;
        internal readonly float hfov;
        internal readonly float* angles;
        internal readonly float* cosines;

        /// <summary>
        ///  How distant the collision from a pixel is from it's direct neighbors in a perperndicular plane at 1u.
        /// </summary>
        internal readonly float step0;

        private RenderCache(int width, int height, float hfov)
        {
            const float TORAD = MathF.PI / 180f;

            this.hfov = hfov;

            float tan = MathF.Tan(TORAD * hfov * 0.5f);
            float per_pixel_step = 2f * tan / width;
            step0 = per_pixel_step;

            colHeight1 = width / (2f * tan);

            // Allocates both angles and cosines pointers at once.
            angles = (float*)Marshal.AllocHGlobal(sizeof(float) * width);
            cosines = (float*)Marshal.AllocHGlobal(sizeof(float) * width);
            float leftPixel = tan - per_pixel_step * 0.5f;
            for (int i = 0; i < width; i++)
            {
                float angle = MathF.Atan(i * per_pixel_step - leftPixel) / TORAD;
                angles[i] = angle;
                cosines[i] = MathF.Cos(TORAD * angle);
            }
        }

        internal static RenderCache* Create(int width, int height, float hfov)
        {
            count++;
            RenderCache* result = (RenderCache*)Marshal.AllocHGlobal(sizeof(RenderCache));
            *result = new(width, height, hfov);
            return result;
        }

        // Mem leak porque nao ta deletando todos os buffers alocados
        internal static void Delete(RenderCache* cache)
        {
            count--;
            cache->Dispose();
            Marshal.FreeHGlobal((IntPtr)cache);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)angles);
            Marshal.FreeHGlobal((IntPtr)cosines);
        }
    }
}
