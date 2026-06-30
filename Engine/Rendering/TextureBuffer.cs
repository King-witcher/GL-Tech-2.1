using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TextureBuffer : IDisposable
    {
        public const int MaxSide = 0x2000;
        public const int PixelSize = 4;

        internal uint* buffer;
        readonly internal int width;
        readonly internal int height;
        readonly internal float widthf;
        readonly internal float heightf;

        public nint Buffer => (nint)buffer;
        public int Height => height;
        public int Width => width;
        public int Size => PixelSize * width * height;

        public TextureBuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("width/height");

            widthf = this.width = GetCeilingPowerOfTwo(width);
            heightf = this.height = GetCeilingPowerOfTwo(height);

            AllocBuffer(this.width, this.height);
        }

        /// <summary>
        /// Create an new Image, copying the data from the pointed bitmap.
        /// </summary>
        public TextureBuffer(int width, int height, uint* buffer) : this(width, height)
        {
            CopyTransposed(buffer, width, height);
        }

        public static TextureBuffer FromColor(Color color)
        {
            TextureBuffer source = new(1, 1);
            source[0, 0] = color;
            return source;
        }

        public static void BufferCopy(TextureBuffer source, TextureBuffer destination)
        {
            if (source.Size > destination.Size)
                throw new ArgumentOutOfRangeException("source");

            System.Buffer.MemoryCopy(
                source: source.buffer,
                destination: destination.buffer,
                destinationSizeInBytes: destination.Size,
                sourceBytesToCopy: source.Size);
        }

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
            get => ((Color*)Buffer)[line + height * column];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => ((Color*)Buffer)[line + height * column] = value;
        }

        public override string ToString()
        {
            return $"{Width}x{Height} {GetType().Name} -> {Buffer}";
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(Buffer);
        }

        private void AllocBuffer(int width, int height)
        {
            buffer = (uint*)Marshal.AllocHGlobal(width * height * PixelSize);
        }

        private int GetCeilingPowerOfTwo(int n)
        {
            for (int i = 1; i <= MaxSide; i <<= 1)
            {
                if (i == n) return i;
                if (i > n) throw new Exception("TextureBuffer's dimensions must be power of two");
            }
            throw new Exception("Texture too big");
        }

        private void CopyTransposed(uint* src, int src_w, int src_h)
        {
            int this_h = height;
            uint* buffer = this.buffer;

            Parallel.For(0, src_h, (line) =>
            {
                for (int col = 0; col < src_w; col++)
                {
                    int src_idx = col + src_w * line;
                    int this_idx = line + this_h * col;
                    uint source = src[src_idx];
                    buffer[this_idx] = source;
                }
            });
        }
    }
}
