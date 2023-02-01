using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct Color : IEquatable<Color>
    {
        // Union
        [FieldOffset(0)] uint uint_value;
        [FieldOffset(0)] int int_value;

        [FieldOffset(3)] byte a;
        [FieldOffset(2)] byte r;
        [FieldOffset(1)] byte g;
        [FieldOffset(0)] byte b;

        public byte R => r;
        public byte G => g;
        public byte B => b;

        // TODO liberar a transparência no A
        public Color(byte red, byte green, byte blue)
        {
            int_value = default;
            uint_value = default;
            a = 0xff; r = red; g = green; b = blue;
        }

        public float Luma => (0.2126f * R * R + 0.7152f * G * G + 0.0722f * B * B) / (255f * 255f);
        public float FastLuma => (0.2126f * R + 0.7152f * G + 0.0722f * B) / (255f);
        public byte Brightness => (byte)((R + G + B) / 3);

        [Obsolete]
        public Color Average(Color rgb)
        {
            rgb.r = (byte)((R + rgb.R) >> 1);
            rgb.g = (byte)((G + rgb.G) >> 1);
            rgb.b = (byte)((B + rgb.B) >> 1);

            return rgb;
        }

        public Color Mix(Color rgb, float factor)
        {
            ushort parcel1, parcel2;

            parcel1 = (ushort)(R * (1 - factor));
            parcel2 = (ushort)(rgb.R * factor);
            rgb.r = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(G * (1 - factor));
            parcel2 = (ushort)(rgb.G * factor);
            rgb.g = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(B * (1 - factor));
            parcel2 = (ushort)(rgb.B * factor);
            rgb.b = (byte)(parcel1 + parcel2);

            return rgb;
        }

        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator *(Color rgb, float factor)
        {
            ulong red = (ulong)(rgb.R * factor);
            if (red > 255)
                red = 255;
            ulong green = (ulong)(rgb.G * factor);
            if (green > 255)
                green = 255;
            ulong blue = (ulong)(rgb.B * factor);
            if (blue > 255)
                blue = 255;

            rgb.r = (byte)(red);
            rgb.g = (byte)(green);
            rgb.b = (byte)(blue);

            return rgb;
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{r}, {g}, {b}>";
        }

        public override bool Equals(object obj)
        {
            if (obj is Color pixel)
                return this == pixel;
            return false;
        }

        public bool Equals(Color other)
        {
            return r == other.r &&
                   g == other.g &&
                   b == other.b;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(r, g, b);
        }

        public static bool operator ==(Color left, Color right)
        {
            return 
                (left.r == right.r) &&
                (left.g == right.g) &&
                (left.b == right.b);
        }

        public static bool operator !=(Color left, Color right)
        {
            return
                (left.r != right.r) |
                (left.g != right.g) |
                (left.b != right.b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(Color rgb) => rgb.uint_value;

        // This allows collors to be represented in hexadecimal like 0xff8000
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(uint rgb) => new Color { uint_value = rgb };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color((byte r, byte g, byte b) components)
        {
            return new Color(components.r, components.g, components.b);
        }
    }
}
