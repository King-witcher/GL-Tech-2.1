using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.Imaging
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ImageData : IDisposable
    {
        static ImageData()
        {
            if (!Environment.Is64BitProcess)
                throw new Exception("GL Tech 2.1 must run as x86-64.");
        }

        public const int BYTES_PER_PIXEL = 4;
        public const PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;

        [FieldOffset(0)] int width;
        [FieldOffset(4)] int height;
        [FieldOffset(8)] uint* uintbuf;
        [FieldOffset(8)] Pixel* pixelbuf;

        [FieldOffset(16)] internal float flt_w;
        [FieldOffset(20)] internal float flt_h;

        public int Height => height;
        public int Width => width;
        public IntPtr Buffer => (IntPtr)uintbuf;
        public Pixel* PixelBuffer => pixelbuf;
        public uint* UintBuffer => uintbuf;

        [Obsolete]
        public ImageData(Bitmap source)
        {
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            using (var clone = source.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                pixelbuf = null;
                uintbuf = (uint*)Marshal.AllocHGlobal(bmpsize);
                System.Buffer.MemoryCopy((void*)bmpdata.Scan0, uintbuf, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            width = source.Width;
            height = source.Height;
            flt_w = source.Width;
            flt_h = source.Height;
        }

        public ImageData(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            this.flt_w = width;
            this.flt_h = height;
            pixelbuf = null; // Assigned by union
            uintbuf = (uint*)Marshal.AllocHGlobal(width * height * sizeof(uint));
        }

        // Em fase de testes
        public static ImageData Clone(Bitmap source)
        {
            ImageData result = default;
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            using Bitmap src32 = source.PixelFormat == PixelFormat.Format32bppArgb ?
                source : source.Clone(rect, PixelFormat.Format32bppArgb);

            var bmpdata = src32.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int bmpsize = bmpdata.Stride * bmpdata.Height;
            result.pixelbuf = default;
            result.uintbuf = (UInt32*)Marshal.AllocHGlobal(bmpsize);
            System.Buffer.MemoryCopy((void*)bmpdata.Scan0, result.uintbuf, bmpsize, bmpsize);
            src32.UnlockBits(bmpdata);

            return result;
        }

        public static void BufferCopy(ImageData source, ImageData destination)
        {
            if (source.Width * source.Height > destination.Width * destination.Height)
                throw new ArgumentOutOfRangeException("source");

            System.Buffer.MemoryCopy(source.UintBuffer, destination.UintBuffer, BYTES_PER_PIXEL * destination.Height * destination.Width, BYTES_PER_PIXEL * source.Height * source.Width);
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
            return new Bitmap(data.width, data.height, BYTES_PER_PIXEL * data.width, PIXEL_FORMAT, data.Buffer);
        }
    }
}
