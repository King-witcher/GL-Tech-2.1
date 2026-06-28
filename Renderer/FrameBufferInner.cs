using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech
{
    public unsafe class FrameBufferInner
    {
        public readonly Color* buffer;
        public readonly int width;
        public readonly int height;
        public readonly float widthf;
        public readonly float heightf;

        public FrameBufferInner(int width, int height)
        {
            buffer = (Color*)Marshal.AllocHGlobal(width * height * sizeof(Color));
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

        ~FrameBufferInner()
        {
            Marshal.FreeHGlobal((nint)buffer);
        }
    }
}
