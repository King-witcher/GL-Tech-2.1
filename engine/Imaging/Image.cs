using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct Image : IDisposable
    {
        static Image()
        {
            if (!Environment.Is64BitProcess)
                throw new Exception("GL Tech 2.1 must run in a x86-64 architecture.");
        }

        public const int DefaultBytesPerPixel = 4;
        public const PixelFormat DefaultPixelFormat = PixelFormat.Format32bppArgb;
        internal static int count = 0;

        readonly internal uint* buffer;
        readonly internal int width;
        readonly internal int height;
        readonly internal float widthf;
        readonly internal float heightf;

        public nint Buffer => (nint) buffer;
        public int Height => height;
        public int Width => width;
        public long MemorySize => DefaultBytesPerPixel * width * height;

        public Image(int width, int height)
        {
            count++;
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("width/height");

            widthf = this.width = width;
            heightf = this.height = height;

            buffer = (uint*) Marshal.AllocHGlobal(width * height * DefaultBytesPerPixel);
        }

        public Image(Bitmap source) : this (source.Width, source.Height)
        {
            // Converts the source into a standarized bits-per-pixel bitmap.
            using Bitmap src32 = source.Clone(DefaultPixelFormat) ??
                throw new ArgumentNullException("source");

            BitmapData lockdata = src32.LockBits();
            System.Buffer.MemoryCopy(
                source: (void*)lockdata.Scan0,
                destination: (void*)buffer,
                sourceBytesToCopy: MemorySize,
                destinationSizeInBytes: MemorySize);

            src32.UnlockBits(lockdata);
        }

        public static Image FromColor(Color color)
        {
            Image source = new(1, 1);
            source[0, 0] = color;
            return source;
        }

        public static void BufferCopy(Image source, Image destination)
        {
            if (source.MemorySize > destination.MemorySize)
                throw new ArgumentOutOfRangeException("source");

            System.Buffer.MemoryCopy(
                source:                 (void*)source.buffer,
                destination:            (void*)destination.buffer,
                destinationSizeInBytes: destination.MemorySize,
                sourceBytesToCopy:      source.MemorySize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Foreach(Func<Color, Color> transformation)
        {
            Color* buffer = (Color*)Buffer;
            int height = Height;
            int width = Width;

            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = transformation(buffer[cur]);
                }
            });
        }

        public void FillWith(Color color)
        {
            int height = Height;
            int width = Width;
            Color* buffer = (Color*)Buffer;

            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = color;
                }
            });
        }

        public Color this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ((Color*)Buffer)[column + Width * line];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => ((Color*)Buffer)[column + Width * line] = value;
        }

        public override string ToString()
        {
            return $"{Width}x{Height} {GetType().Name} -> {Buffer}";
        }

        public void Dispose()
        {
            count--;
            Marshal.FreeHGlobal(Buffer);
        }

        public static explicit operator Bitmap(Image data)
        {
            return new Bitmap(
                data.Width,
                data.Height,
                DefaultBytesPerPixel * data.Width,
                DefaultPixelFormat,
                data.Buffer
            );
        }
    }
}
