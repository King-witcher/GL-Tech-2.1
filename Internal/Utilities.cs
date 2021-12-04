using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Engine
{
    internal static unsafe class Utilities
    {
        internal const float ToRad = (float)Math.PI / 180f;
        internal const float ToDegree = 180f / (float)Math.PI;

        internal static void Clip<T>(ref T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            else if (value.CompareTo(min) < 0)
                value = min;
        }

        public static Rectangle GetRectangle(this Bitmap bitmap)
        {
            return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        public static BitmapData LockBits(this Bitmap bitmap)
        {
            return bitmap.LockBits(bitmap.GetRectangle(), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        }

        public static Bitmap Clone(this Bitmap bitmap, PixelFormat format)
        {
            return bitmap.Clone(bitmap.GetRectangle(), format);
        }
    }
}
