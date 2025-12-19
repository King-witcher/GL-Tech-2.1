using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal Image source; //Propositalmente salvo por valor
        public float hoffset;
        public float hrepeat;
        public float voffset;
        public float vrepeat;

        public Image Image => source;

        public Texture(Image source, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.source = source;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        public static Texture NullTexture => default;

        public bool IsNull => source.Buffer == IntPtr.Zero;

        public static Texture FromColor(Color color, out Image source)
        {
            source = Image.FromColor(color);
            return new(source);
        }

        // Mapeia um pixel da textura baseado em uma proporção x e uma y que vai de 0 a 1
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        internal unsafe uint MapNearest(float hratio, float vratio)
        {
            unchecked
            {
                int x = (int)(source.widthf * (hrepeat * hratio + hoffset)) % source.width;
                int y = (int)(source.heightf * (vrepeat * vratio + voffset)) % source.height;

                uint color = source.buffer[source.width * y + x];

                return color;
            }
        }
    }
}