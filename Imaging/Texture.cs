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

        public Texture(Image buffer, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.source = buffer;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        // Mapeia um pixel da textura baseado em uma proporção x e uma y que vai de 0 a 1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)
        {
            // This is the most performance critical segment of code in the entire engine
            int x = (int)(source.flt_width * (hrepeat * hratio + hoffset)) % source.Width;
            int y = (int)(source.flt_height * (vrepeat * vratio + voffset)) % source.Height;
            //if (source.Width == 1024)
            //    col2 = source.UintBuffer[262144];

            uint color = source.UintBuffer[source.Width * y + x];

            //if (source.Width == 1024)
            //    System.Console.WriteLine(x + " " + y + " ok");

            return color;
        }
    }
}
