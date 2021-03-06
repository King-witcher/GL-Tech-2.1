using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;
using Engine.World;

namespace Engine.Data
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SSprite
    {
        internal Vector position;
        internal Texture material;
        internal SSprite* list_next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SSprite* Create(Vector position, Texture material)
        {
            SSprite* result = (SSprite*)Marshal.AllocHGlobal(sizeof(SSprite));
            result->position = position;
            result->material = material;
            result->list_next = null;
            return result;
        }

        public static implicit operator SSprite(Sprite sprite) =>
            *sprite.unmanaged;

        public static void Delete(SSprite* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
