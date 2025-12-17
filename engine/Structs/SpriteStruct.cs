using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using GLTech.Imaging;
using GLTech.World;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SpriteStruct
    {
        internal Vector position;
        internal Texture material;
        internal SpriteStruct* list_next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SpriteStruct* Create(Vector position, Texture material)
        {
            SpriteStruct* result = (SpriteStruct*)Marshal.AllocHGlobal(sizeof(SpriteStruct));
            result->position = position;
            result->material = material;
            result->list_next = null;
            return result;
        }

        public static implicit operator SpriteStruct(Sprite sprite) =>
            *sprite.unmanaged;

        public static void Delete(SpriteStruct* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
