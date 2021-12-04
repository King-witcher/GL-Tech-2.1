using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SCamera
    {
        internal Vector position;
        internal float rotation; //MUST be 0 <= x < 360

        static internal SCamera* Create(Vector position, float rotation) // a little bit optimizable
        {
            SCamera* result = (SCamera*)Marshal.AllocHGlobal(sizeof(SCamera));
            result->position = position;
            result->rotation = rotation;
            return result;
        }

        static internal void Delete(SCamera* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
