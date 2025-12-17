using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CameraStruct
    {
        internal Vector position;
        internal Vector direction;
        internal float z;

        static internal CameraStruct* Create(Vector position, Vector direction)
        {
            CameraStruct* result = (CameraStruct*)Marshal.AllocHGlobal(sizeof(CameraStruct));
            result->position = position;
            result->direction = direction;
            result->z = 0.5f;
            return result;
        }

        static internal void Delete(CameraStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
