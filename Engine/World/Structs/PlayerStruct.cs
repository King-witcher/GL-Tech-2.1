using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine;

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
            PlayerStruct* result = Memory.Alloc<PlayerStruct>(alignment: 16);
            result->position = position;
            result->direction = direction;
            result->height = 0.5f;
            return result;
        }

        static public void Delete(PlayerStruct* item)
        {
            Memory.Free(item);
        }
    }
}
