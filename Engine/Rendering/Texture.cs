using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal TextureBuffer source; //Propositalmente salvo por valor
        public float hoffset;
        public float hrepeat;
        public float voffset;
        public float vrepeat;

        public TextureBuffer Image => source;

        public Texture(TextureBuffer source, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.source = source;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        public static Texture NullTexture => default;

        public bool IsNull => source.Buffer == IntPtr.Zero;

        public static Texture FromColor(Color color, out TextureBuffer source)
        {
            source = TextureBuffer.FromColor(color);
            return new(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        internal uint MapNearest(float hratio, float vratio)
        {
            unchecked
            {
                int x = (int)(source.widthf * MathF.FusedMultiplyAdd(hrepeat, hratio, hoffset)) & (source.width - 1);
                int y = (int)(source.heightf * MathF.FusedMultiplyAdd(vrepeat, vratio, voffset)) & (source.height - 1);

                uint color = source.buffer[source.height * x + y];

                return color;
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        internal readonly int ResolveX(float hratio)
        {
            unchecked
            {
                return (int)(source.widthf * MathF.FusedMultiplyAdd(hrepeat, hratio, hoffset)) & (source.width - 1);
            }
        }


        // Resolves the base pointer of the texture column for a fixed horizontal
        // ratio. hratio is constant across a wall column, so both x and the
        // column base (height * x) are computed once here instead of per pixel.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        internal readonly uint* ResolveColumn(float hratio)
        {
            unchecked
            {
                return source.buffer + source.height * ResolveX(hratio);
            }
        }


        // Samples a pixel from a pre-resolved texture column. Only the vertical
        // index is computed per pixel; the column base was hoisted out of the loop.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        internal readonly uint MapColumn(uint* column, float vratio)
        {
            unchecked
            {
                int y = (int)(source.heightf * MathF.FusedMultiplyAdd(vrepeat, vratio, voffset)) & (source.height - 1);
                return column[y];
            }
        }
    }
}
