using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderCache : IDisposable
    {
        // Height that a column at 1 unit of distance from the spectator should be drawn on the screen.
        internal readonly float colHeight1;
        internal readonly float FOV;
        internal readonly float* angles;
        internal readonly float* cosines;
        internal readonly float* fall_dists;
        internal readonly float* fall_factors;

        internal RenderCache(int width, int height, float FOV = 90f)
        {
            const double TORAD = Math.PI / 180f;

            this.FOV = FOV;

            double tan = Math.Tan(TORAD * FOV / 2f);
            colHeight1 = width / (float)(2.0 * tan);

            // Allocates both angles and cosines pointers at once.
            angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * width);
            cosines = angles + width;

            double step = 2.0 * tan / width;
            double leftPixel = tan - step / 2.0;
            for (int i = 0; i < width; i++)
            {
                float angle = (float)(Math.Atan(i * step - leftPixel) / TORAD);
                angles[i] = angle;
                cosines[i] = (float)(Math.Cos(TORAD * angle));
            }

            // Fall distances
            fall_dists = (float*)Marshal.AllocHGlobal(sizeof(float) * height >> 1);
            fall_factors = (float*)Marshal.AllocHGlobal(sizeof(float) * height >> 1);
            float pre_dist = width / (2 * height * (float)Math.Tan(Util.ToRad * FOV / 2f));
            for (int line = 0; line < height >> 1; line++)
            {
                float post_dist = pre_dist * (line - 0.5f) / (height / 2 - line + 0.5f);
                float fall_dist = pre_dist + post_dist;
                fall_dists[line] = pre_dist + post_dist;
                fall_factors[line] = fall_dist / pre_dist;
            }
        }

        internal static RenderCache* Create(int width, int height, float FOV = 90f)
        {
            RenderCache* result = (RenderCache*)Marshal.AllocHGlobal(sizeof(RenderCache));
            *result = new RenderCache(width, height, FOV);
            return result;
        }

        // Mem leak porque nao ta deletando todos os buffers alocados
        internal static void Delete(RenderCache* cache)
        {
            cache->Dispose();
            Marshal.FreeHGlobal((IntPtr)cache);
        }

        public void Dispose()
        {
            // Releases both sines and cosines.
            Marshal.FreeHGlobal((IntPtr)angles);
            Marshal.FreeHGlobal((IntPtr)fall_dists);
        }
    }
}
