using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2.Imaging
{
    [NativeCppClass] [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal PixelBuffer buffer; //Propositalmente salvo por valor
        internal float hoffset;
        internal float hrepeat;
        internal float voffset;
        internal float vrepeat;

        public Texture(PixelBuffer buffer, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.buffer = buffer;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        // Mapeia um pixel da textura baseado em uma proporção x e uma y que vai de 0 a 1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)
        {
            // This is the most performance critical segment of code int the entire engine
            int x = (int)(buffer.width_float * (hrepeat * hratio + hoffset)) % buffer.Width;
            int y = (int)(buffer.height_float * (vrepeat * vratio + voffset)) % buffer.Height;
            return buffer.Uint0[buffer.Width * y + x];
        }
    }
}
