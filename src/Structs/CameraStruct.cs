using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CameraStruct
    {
        internal Vector position;
        internal float rotation; //MUST be 0 <= x < 360

        static internal CameraStruct* Create(Vector position, float rotation) // a little bit optimizable
        {
            CameraStruct* result = (CameraStruct*)Marshal.AllocHGlobal(sizeof(CameraStruct));
            result->position = position;
            result->rotation = rotation;
            return result;
        }

        static internal void Delete(CameraStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
