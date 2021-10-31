using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2.Imaging
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color
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

        public Color(byte red, byte green, byte blue)
        {
            rgb = 0;
            a = 255;
            r = red;
            g = green;
            b = blue;
        }

        public float Luma => (0.2126f * r * r + 0.7152f * g * g + 0.0722f * b * b) / (255f * 255f);

        public float FastLuma => (0.2126f * r + 0.7152f * g + 0.0722f * b) / (255f);

        public byte Brightness => (byte)((r + g + b) / 3);

        public byte R => r;

        public byte G => g;

        public byte B => b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color Average(Color rgb)
        {
            rgb.r = (byte)((r + rgb.r) >> 1);
            rgb.g = (byte)((g + rgb.g) >> 1);
            rgb.b = (byte)((b + rgb.b) >> 1);

            return rgb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color Mix(Color rgb, float factor)
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

        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator *(Color rgb, float factor)
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

            rgb.r = (byte)(red);
            rgb.g = (byte)(green);
            rgb.b = (byte)(blue);

            return rgb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(Color rgb) => rgb.rgb;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(uint rgb) => new Color { rgb = rgb };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color((byte r, byte g, byte b) components)
        {
            return new Color(components.r, components.g, components.b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Color(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Drawing.Color(Color rgb)
        {
            return System.Drawing.Color.FromArgb(255, rgb.r, rgb.g, rgb.b);
        }
    }
}
