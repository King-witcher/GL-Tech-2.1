using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PlayerStruct
    {
        public Vector position;
        public Vector direction;
        public float height;

        static public PlayerStruct* Create(Vector position, Vector direction)
        {
            PlayerStruct* result = (PlayerStruct*)Marshal.AllocHGlobal(sizeof(PlayerStruct));
            result->position = position;
            result->direction = direction;
            result->height = 0.5f;
            return result;
        }

        static public void Delete(PlayerStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
