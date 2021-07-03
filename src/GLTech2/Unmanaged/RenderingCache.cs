using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    /// Stores important cache data to the renderer that can be shared with native modes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderingCache : IDisposable
    {
        // Height that a column at 1 unit of distance from the spectator should be drawn on the screen.
        internal readonly float colHeight1;
        internal readonly float FOV;
        internal readonly float* angles;
        internal readonly float* cosines;

        internal RenderingCache(int width, int height, float FOV = 90f)
        {
            const double TORAD = Math.PI / 180f;

            this.FOV = FOV;

            double tan = Math.Tan(TORAD * FOV / 2f);
            colHeight1 = width / (float)(2.0 * tan);

            // Allocates both angles and cosines pointers at once.
            angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * width);
            cosines = angles + width;

            double step = 2 * tan / (width - 1);
            for (int i = 0; i < width; i++)
            {
                float angle = (float)(Math.Atan(i * step - tan) / TORAD);
                angles[i] = angle;
                cosines[i] = (float)(Math.Cos(TORAD * angle));
            }
        }

        internal static RenderingCache* Create(int width, int height, float FOV = 90f)
        {
            RenderingCache* result = (RenderingCache*)Marshal.AllocHGlobal(sizeof(RenderingCache));
            *result = new RenderingCache(width, height, FOV);
            return result;
        }

        internal static void Delete(RenderingCache* cache)
        {
            cache->Dispose();
            Marshal.FreeHGlobal((IntPtr)cache);
        }

        public void Dispose()
        {
            // Releases both sines and cosines.
            Marshal.FreeHGlobal((IntPtr)angles);
        }
    }
}
