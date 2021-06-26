using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SpriteData
    {
        internal Vector position;
        internal Texture material;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SpriteData* Create(Vector position, Texture material)
        {
            SpriteData* result = (SpriteData*)Marshal.AllocHGlobal(sizeof(SpriteData));
            result->position = position;
            result->material = material;
            return result;
        }

        public static implicit operator SpriteData(Sprite sprite) =>
            *sprite.unmanaged;

        public static void Delete(SpriteData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
