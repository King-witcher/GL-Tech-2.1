using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    /// Represents a texture that can be attached to a renderable object.
    /// </summary>
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal PixelBuffer buffer; //Propositalmente salvo por valor
        internal float hoffset;
        internal float hrepeat;
        internal float voffset;
        internal float vrepeat;

        /// <summary>
        /// Gets a new instance of Texture given a PixelBuffer.
        /// </summary>
        /// <param name="buffer">A texture</param>
        /// <param name="hoffset">Horizontal offset of the texture</param>
        /// <param name="hrepeat">How many times the texture is repeated horizontally</param>
        /// <param name="voffset">Not implemented yet</param>
        /// <param name="vrepeat">Not implemented yet</param>
        public Texture(PixelBuffer buffer, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.buffer = buffer;
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)
        {
            // Critical performance impact
            int x = (int)(buffer.width_float * (hrepeat * hratio + hoffset)) % buffer.width;
            int y = (int)(buffer.height_float * (vrepeat * vratio + voffset)) % buffer.height;
            return buffer.uint0[buffer.width * y + x];
        }
    }
}
