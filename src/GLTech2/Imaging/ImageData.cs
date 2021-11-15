using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.Imaging
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe readonly struct ImageData : IDisposable
    {
        static ImageData()
        {
            if (!Environment.Is64BitProcess)
                throw new Exception("GL Tech 2.1 must run as x86-64.");
        }

        public const int DEFAULT_BPP = 4;
        public const PixelFormat DEFAULT_PIXEL_FORMAT = PixelFormat.Format32bppArgb;

        [FieldOffset(0)] readonly int width;
        [FieldOffset(4)] readonly int height;
        [FieldOffset(8)] readonly uint* uint_buffer;
        [FieldOffset(8)] readonly Pixel* pixel_buffer;

        [FieldOffset(16)] readonly internal float flt_width;
        [FieldOffset(20)] readonly internal float flt_height;

        public int Height => height;
        public int Width => width;
        public IntPtr Buffer => (IntPtr)uint_buffer;
        public Pixel* PixelBuffer => pixel_buffer;
        public uint* UintBuffer => uint_buffer;
        public long Size => DEFAULT_BPP * width * height;

        [Obsolete]
        public ImageData(Bitmap source)
        {
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            using (var clone = source.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                pixel_buffer = null;
                uint_buffer = (uint*)Marshal.AllocHGlobal(bmpsize);
                System.Buffer.MemoryCopy((void*)bmpdata.Scan0, uint_buffer, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            width = source.Width;
            height = source.Height;
            flt_width = source.Width;
            flt_height = source.Height;
        }

        public ImageData(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            this.flt_width = width;
            this.flt_height = height;
            pixel_buffer = null; // Assigned by union
            uint_buffer = (uint*)Marshal.AllocHGlobal(width * height * DEFAULT_BPP);
        }

        // Em fase de testes
        public static ImageData Clone(Bitmap source)
        {
            // Allocates the clone
            ImageData clone = new(source.Width, source.Height);

            // Clones the source if it's format is different from the expected and releases at the end.
            using var src32 = source.PixelFormat == DEFAULT_PIXEL_FORMAT ?
                source: source.Clone(DEFAULT_PIXEL_FORMAT) ??
                throw new ArgumentNullException("source");

            // Copies each byte from the bitmap to the clone.
            BitmapData lockdata = src32.LockBits();
            System.Buffer.MemoryCopy(
                source:                 (void*)lockdata.Scan0,
                destination:            clone.uint_buffer,
                sourceBytesToCopy:      clone.Size,
                destinationSizeInBytes: clone.Size);

            src32.UnlockBits(lockdata);
            return clone;
        }

        public static void BufferCopy(ImageData source, ImageData destination)
        {
            if (source.Width * source.Height > destination.Width * destination.Height)
                throw new ArgumentOutOfRangeException("source");
            System.Buffer.MemoryCopy(source.UintBuffer, destination.UintBuffer, DEFAULT_BPP * destination.Height * destination.Width, DEFAULT_BPP * source.Height * source.Width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Foreach(Func<Pixel, Pixel> transformation)
        {
            int height = this.Height;
            int width = this.Width;
            uint* buffer = this.UintBuffer;

            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = transformation(buffer[cur]);
                }
            });
        }

        public Pixel this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PixelBuffer[column + Width * line];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => PixelBuffer[column + Width * line] = value;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(Buffer);
        }

        public static explicit operator ImageData(Bitmap bitmap)
        {
            return new ImageData(bitmap);
        }

        public static explicit operator Bitmap(ImageData data)
        {
            return new Bitmap(data.width, data.height, DEFAULT_BPP * data.width, DEFAULT_PIXEL_FORMAT, data.Buffer);
        }
    }
}
