using GLTech;
using System.Runtime.CompilerServices;

namespace Engine.Window
{
    internal unsafe class RenderTarget
    {
        public readonly Color* buffer;
        public readonly int width;
        public readonly int height;
        public readonly float widthf;
        public readonly float heightf;

        public RenderTarget(int width, int height)
        {
            buffer = Memory.Alloc<Color>(width * height, alignment: 64); // Cache line: the framebuffer is written scanline by scanline.
            widthf = this.width = width;
            heightf = this.height = height;
        }

        public Color this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (buffer)[column + width * line];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => (buffer)[column + width * line] = value;
        }

        ~RenderTarget()
        {
            Memory.Free(buffer);
        }
    }
}
