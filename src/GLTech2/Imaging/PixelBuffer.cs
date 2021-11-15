using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.Imaging
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct PixelBuffer : IDisposable
    {
        [FieldOffset(0)] int width;
        [FieldOffset(4)] int height;
        [FieldOffset(8)] internal float width_float;
        [FieldOffset(12)] internal float height_float;
        [FieldOffset(16)] uint* uint0;
        [FieldOffset(16)] Color* rgb0;  // Union

        public Color this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RGB0[column + Width * line];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => RGB0[column + Width * line] = value;
        }

        public const int BYTES_PER_PIXEL = 4;

        public int Height => height;

        public int Width => width;

        public IntPtr Scan0 => (IntPtr)uint0;

        public Color* RGB0 => rgb0;

        public uint* Uint0 => uint0;

        public PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        public PixelBuffer(Bitmap source)
        {
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            using (var clone = source.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                rgb0 = null; // Assigned next line
                uint0 = (UInt32*)Marshal.AllocHGlobal(bmpsize);
                Buffer.MemoryCopy((void*)bmpdata.Scan0, uint0, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            width = source.Width;
            height = source.Height;
            width_float = source.Width;
            height_float = source.Height;
        }

        public PixelBuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            this.width_float = width;
            this.height_float = height;
            rgb0 = null; // Assigned by union
            uint0 = (uint*)Marshal.AllocHGlobal(width * height * sizeof(uint));
        }

        public static void BufferCopy(PixelBuffer source, PixelBuffer destination)
        {
            if (source.Width * source.Height > destination.Width * destination.Height)
                throw new ArgumentOutOfRangeException("source");

            Buffer.MemoryCopy(source.Uint0, destination.Uint0, BYTES_PER_PIXEL * destination.Height * destination.Width, BYTES_PER_PIXEL * source.Height * source.Width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Foreach(Func<Color, Color> transformation)
        {
            int height = this.Height;
            int width = this.Width;
            uint* buffer = this.Uint0;

            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = transformation(buffer[cur]);
                }
            });
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(Scan0);
        }

        public static explicit operator PixelBuffer(Bitmap bitmap)
        {
            return new PixelBuffer(bitmap);
        }

        public static explicit operator Bitmap(PixelBuffer texture)
        {
            return new Bitmap(texture.width, texture.height, BYTES_PER_PIXEL * texture.width, texture.PixelFormat, texture.Scan0);
        }
    }
}
