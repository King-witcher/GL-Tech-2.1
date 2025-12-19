using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CameraStruct
    {
        public Vector position;
        public Vector direction;
        public float z;

        static public CameraStruct* Create(Vector position, Vector direction)
        {
            CameraStruct* result = (CameraStruct*)Marshal.AllocHGlobal(sizeof(CameraStruct));
            result->position = position;
            result->direction = direction;
            result->z = 0.5f;
            return result;
        }

        static public void Delete(CameraStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
