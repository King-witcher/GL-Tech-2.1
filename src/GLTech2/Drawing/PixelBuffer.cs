using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.Drawing
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe readonly struct PixelBuffer : IDisposable
    {
        [FieldOffset(0)]
        internal readonly int width;
        [FieldOffset(4)]
        internal readonly int height;

        // Theese are stored as float due to small optimizations.
        [FieldOffset(8)]
        internal readonly float width_float;
        [FieldOffset(12)]
        internal readonly float height_float;

        // Union
        [FieldOffset(16)]
        internal readonly uint* uint0;
        [FieldOffset(16)]
        internal readonly RGB* rgb0;

        public RGB this[int column, int line]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => rgb0[column + width * line];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => rgb0[column + width * line] = value;
        }

        public int Height => height;

        public int Width => width;

        public IntPtr Scan0 => (IntPtr)uint0;

        public RGB* RGB0 => rgb0;

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

        public void Clone(PixelBuffer source)
        {
            if (width != source.width || height != source.height)
                throw new ArgumentException("Buffers must have the same size.");
            Buffer.MemoryCopy(source.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Foreach(Func<RGB, RGB> transformation)
        {
            int height = this.height;
            int width = this.width;
            uint* buffer = this.uint0;

            Parallel.For(0, width, (x) =>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void FastClone(PixelBuffer buffer)
        {
            Buffer.MemoryCopy(buffer.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        public static explicit operator PixelBuffer(Bitmap bitmap)
        {
            return new PixelBuffer(bitmap);
        }

        public static implicit operator Bitmap(PixelBuffer texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }
    }
}
