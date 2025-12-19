using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SpriteStruct
    {
        public Vector position;
        public Texture material;
        public SpriteStruct* list_next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpriteStruct* Create(Vector position, Texture material)
        {
            SpriteStruct* result = (SpriteStruct*)Marshal.AllocHGlobal(sizeof(SpriteStruct));
            result->position = position;
            result->material = material;
            result->list_next = null;
            return result;
        }

        public static void Delete(SpriteStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
