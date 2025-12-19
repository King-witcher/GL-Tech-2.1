using System.Drawing;
using System.Drawing.Imaging;

namespace GLTech
{
    internal static unsafe class InternalUtils
    {
        internal const float ToRad = (float)System.Math.PI / 180f;
        internal const float ToDegree = 180f / (float)System.Math.PI;

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

        public static float Random()
        {
            Random r = new Random();
            return (float)r.NextDouble();
        }
    }

    public static class Utils
    {
        public static unsafe Image GetImageFromBitmap(Bitmap source)
        {
            // Converts the source into a standarized bits-per-pixel bitmap.
            using Bitmap src32 = source.Clone(PixelFormat.Format32bppArgb) ??
                throw new ArgumentNullException("source");

            Image image = new Image(source.Width, source.Height);

            BitmapData lockdata = src32.LockBits();
            System.Buffer.MemoryCopy(
                source: (void*)lockdata.Scan0,
                destination: (void*)image.Buffer,
                sourceBytesToCopy: image.Size,
                destinationSizeInBytes: image.Size);

            src32.UnlockBits(lockdata);
            return image;
        }
    }
}
