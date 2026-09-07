using GLTech;
using System.Runtime.CompilerServices;

namespace Engine.Rendering
{
    internal unsafe readonly struct SwapchainImage
    {
        public readonly Color* buffer;
        public readonly int width;
        public readonly int height;
        public readonly float widthf;
        public readonly float heightf;

        public SwapchainImage(Color* buffer, int width, int height)
        {
            this.buffer = buffer;
            widthf = this.width = width;
            heightf = this.height = height;
        }

        [Obsolete]
        public Color this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (buffer)[column + width * line];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => (buffer)[column + width * line] = value;
        }
    }
}
