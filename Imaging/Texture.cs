using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Imaging
{
    [NativeCppClass] [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal Image source; //Propositalmente salvo por valor
        internal float hoffset;
        internal float hrepeat;
        internal float voffset;
        internal float vrepeat;

        public Texture(Image source, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.source = source;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        public static Texture NullTexture => default;

        public static Texture FromColor(Color color, out Image source)
        {
            source = new(1, 1);
            source[0, 0] = color;
            return new(source);
        }

        // Mapeia um pixel da textura baseado em uma proporção x e uma y que vai de 0 a 1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)
        {
            // This is the most performance critical segment of code in the entire engine
            int x = (int)(source.flt_width * (hrepeat * hratio + hoffset)) % source.Width;
            int y = (int)(source.flt_height * (vrepeat * vratio + voffset)) % source.Height;

            uint color = ((uint*)source.Buffer)[source.Width * y + x];

            return color;
        }
    }
}
