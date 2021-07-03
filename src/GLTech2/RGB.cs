using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    /// <summary>
    /// Represents a 32 bits RGB color.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RGB
    {
        //Union
        [FieldOffset(0)]
        internal uint rgb;
        [FieldOffset(0)]
        internal byte b;
        [FieldOffset(1)]
        internal byte g;
        [FieldOffset(2)]
        internal byte r;
        [FieldOffset(3)]
        private byte a;

        public RGB(byte red, byte green, byte blue)
		{
            rgb = 0;
            a = 255;
            r = red;
            g = green;
            b = blue;
		}

        /// <summary>
        /// Gets the luma of the pixel.
        /// </summary>
        public float Luma => (0.2126f * r * r + 0.7152f * g * g + 0.0722f * b * b) / (255f * 255f);

        /// <summary>
        /// Gets a less precise but faster calculated value of the luma of the color.
        /// </summary>
        public float FastLuma => (0.2126f * r + 0.7152f * g + 0.0722f * b) / (255f);

        /// <summary>
        /// Gets the 0-255 brightness of the pixel.
        /// </summary>
        public byte Brightness => (byte)((r + g + b) / 3);

        /// <summary>
        /// The red component
        /// </summary>
        public byte R => r;

        /// <summary>
        /// The green component
        /// </summary>
        public byte G => g;

        /// <summary>
        /// The blue component
        /// </summary>
        public byte B => b;

        /// <summary>
        /// Gets the average between this and another pixel.
        /// </summary>
        /// <param name="rgb">Another pixel</param>
        /// <returns>The average between this and the other pixel.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RGB Average(RGB rgb)
        {
            rgb.r = (byte)((r + rgb.r) >> 1);
            rgb.g = (byte)((g + rgb.g) >> 1);
            rgb.b = (byte)((b + rgb.b) >> 1);

            return rgb;
        }

        /// <summary>
        /// Gets a weighted average between this and another pixel.
        /// </summary>
        /// <param name="rgb">Another pixel</param>
        /// <param name="factor">Weight of the other pixel</param>
        /// <returns>The weighted average between this and the other pixel</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RGB Mix(RGB rgb, float factor)
        {
            ushort parcel1, parcel2;

            parcel1 = (ushort)(r * (1 - factor));
            parcel2 = (ushort)(rgb.r * factor);
            rgb.r = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(g * (1 - factor));
            parcel2 = (ushort)(rgb.g * factor);
            rgb.g = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(b * (1 - factor));
            parcel2 = (ushort)(rgb.b * factor);
            rgb.b = (byte)(parcel1 + parcel2);

            return rgb;
        }

        /// <summary>
        /// Multiplies the intensity of a pixel by a certain amount.
        /// </summary>
        /// <param name="rgb">The pixel</param>
        /// <param name="factor">The amount</param>
        /// <returns>Resultint color</returns>
        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator *(RGB rgb, float factor)
        {
            ulong red = (ulong)(rgb.r * factor);
            if (red > 255)
                red = 255;
            ulong green = (ulong)(rgb.g * factor);
            if (green > 255)
                green = 255;
            ulong blue = (ulong)(rgb.b * factor);
            if (blue > 255)
                blue = 255;

            rgb.r = (byte) (red);
            rgb.g = (byte) (green);
            rgb.b = (byte) (blue);

            return rgb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(RGB rgb) => rgb.rgb;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RGB(uint rgb) => new RGB{rgb = rgb};

        /// <summary>
        /// Implicitly casts a tuple into a RGB struct.
        /// </summary>
        /// <param name="components">The r, g, b components.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RGB((byte r, byte g, byte b) components)
        {
            return new RGB(components.r, components.g, components.b);
        }

        /// <summary>
        /// Explicitly converts a System.Drawing.Color instance to RGB. RGB does not store the A component.
        /// </summary>
        /// <param name="color">The color to be converted</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator RGB(System.Drawing.Color color)
        {
            return new RGB(color.R, color.G, color.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Drawing.Color(RGB rgb)
        {
            return System.Drawing.Color.FromArgb(255, rgb.r, rgb.g, rgb.b);
        }
    }
}
