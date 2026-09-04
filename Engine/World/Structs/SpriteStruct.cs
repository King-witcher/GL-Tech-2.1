using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SpriteStruct
    {
        public Vector position;
        public Texture material;
        public SpriteStruct* list_next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpriteStruct* Create(Vector position, Texture material)
        {
            SpriteStruct* result = Memory.Alloc<SpriteStruct>(alignment: 16);
            result->position = position;
            result->material = material;
            result->list_next = null;
            return result;
        }

        public static void Delete(SpriteStruct* item)
        {
            Memory.Free(item);
        }
    }
}
