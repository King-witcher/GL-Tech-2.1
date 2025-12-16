using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderCache
    {
        // Height that a column at 1 unit of distance from the spectator should be drawn on the screen.
        internal readonly float colHeight1;
        internal readonly float step0;

        private RenderCache(float hfov_deg, float width)
        {
            const float DEG_TO_RAD = MathF.PI / 180f;
            var tan = MathF.Tan(DEG_TO_RAD * hfov_deg * .5f);
            step0 = 2f * tan / width;
            colHeight1 = width / (2f * tan);
        }

        internal static RenderCache* Create(float hfov, float width)
        {
            RenderCache* result = (RenderCache*)Marshal.AllocHGlobal(sizeof(RenderCache));
            *result = new(hfov, width);
            return result;
        }

        internal static void Delete(RenderCache* cache)
        {
            Marshal.FreeHGlobal((IntPtr)cache);
        }
    }
}
