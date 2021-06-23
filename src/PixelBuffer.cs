using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GLTech2
{
    /// <summary>
    /// Represents a 32-bits-per-pixel pixel buffer.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct PixelBuffer : IDisposable
    {
        [FieldOffset(0)]
        internal int width;
        [FieldOffset(4)]
        internal int height;

        // Theese are stored as float due to small optimizations.
        [FieldOffset(8)]
        internal float height_float;
        [FieldOffset(12)]
        internal float width_float;

        // Union
        [FieldOffset(16)]
        internal uint* uint0;
        [FieldOffset(16)]
        internal RGB* rgb0;

        /// <summary>
        /// Gets a RGB pixel given its cordinates.
        /// </summary>
        /// <param name="column">The column</param>
        /// <param name="line">The line</param>
        /// <returns></returns>
        public RGB this[int column, int line]
        {
            get => rgb0[column + width * line];
            set => rgb0[column + width * line] = value;
        }

        /// <summary>
        /// Gets the height of the buffer.
        /// </summary>
        public int Height => height;

        /// <summary>
        /// Gets the witdh of the buffer.
        /// </summary>
        public int Width => width;

        /// <summary>
        /// Gets an IntPtr that represents a pointer to the first pixel of the buffer.
        /// </summary>
        public IntPtr Scan0 => (IntPtr)uint0;

        /// <summary>
        /// Gets the pointer to first RGB pixel from the buffer.
        /// </summary>
        public RGB* RGB0 => rgb0;

        /// <summary>
        /// Gets the pointer to the first pixel of the buffer as uint.
        /// </summary>
        public uint* Uint0 => uint0;

        /// <summary>
        /// PixelFormat.Format32bppArgb
        /// </summary>
        public PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        /// <summary>
        /// Gets a new instance of PixelBuffer equivalent to the specified bitmap.
        /// </summary>
        /// <remarks>
        /// Instantiating a new PixelBuffer is not a boxing, but a cloning operation and the buffer must be disposed when unused.
        /// </remarks>
        /// <param name="source">The source bitmap</param>
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

        /// <summary>
        /// Gets a new empty instance of PixelBuffer given it's dimentions.
        /// </summary>
        /// <param name="width">Witdth</param>
        /// <param name="height">Height</param>
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

        /// <summary>
        /// Clones all data from another buffer.
        /// </summary>
        /// <param name="source">The source buffer</param>
        public void Clone(PixelBuffer source)
		{
            if (width != source.width || height != source.height)
                throw new ArgumentException("Buffers must have the same size.");
            Buffer.MemoryCopy(source.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        /// <summary>
        /// Runs, in parallel, a RGB -> RGB function into every pixel of the buffer.
        /// </summary>
        /// <param name="transformation"></param>
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Releases all unmanaged data.
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(Scan0);
        }

        internal void FastClone(PixelBuffer buffer)
        {
            Buffer.MemoryCopy(buffer.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        /// <summary>
        /// Explicitly casts from System.Drawing.Bitmap to Texture.
        /// </summary>
        /// <param name="bitmap">Bitmap to be cast</param>
        public static explicit operator PixelBuffer(Bitmap bitmap)
        {
            return new PixelBuffer(bitmap);
        }

        /// <summary>
        /// Implicitly casts from Texture to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="texture"></param>
        public static implicit operator Bitmap(PixelBuffer texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }
    }
}
