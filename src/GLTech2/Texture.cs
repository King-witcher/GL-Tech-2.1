using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    /// Represents a texture that can be attached to a renderable object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal float hoffset;
        internal float hrepeat;
        internal PixelBuffer buffer; //Propositalmente salvo por valor
        internal float voffset; //Ainda não implementado
        internal float vrepeat; //Ainda não implmenetado

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
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.buffer = buffer;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)
        {
            // Critical performance impact
            int x = (int)(buffer.width_float * (hrepeat * hratio + hoffset)) % buffer.width;
            int y = (int)(buffer.height_float * vratio);//% buffer.height;
            return buffer.uint0[buffer.width * y + x];
        }

        /// <summary>
        /// Implicitly casts a PixelBuffer instance into a Texture.
        /// </summary>
        /// <param name="buffer">Buffer to be cast</param>
        [Obsolete]
        public static implicit operator Texture(PixelBuffer buffer) =>
            new Texture(buffer);

        /// <summary>
        /// Explicitly casts a System.Drawing.Bitmap into a Texture.
        /// </summary>
        /// <param name="bitmap">Bitmap to be cast</param>
        /// <remarks>
        ///     Note that casting bitmaps directly to Texture will anonymously create a new Texture object that uses unmanaged resources each time it's done, but you won't be able to dispose them.
        /// </remarks>
        [Obsolete]
        public static explicit operator Texture(Bitmap bitmap) =>
            new Texture(new PixelBuffer(bitmap));
    }
}
