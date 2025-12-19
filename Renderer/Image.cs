using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct Image : IDisposable
    {
        public const int PixelSize = 4;

        readonly internal uint* buffer;
        readonly internal int width;
        readonly internal int height;
        readonly internal float widthf;
        readonly internal float heightf;

        public nint Buffer => (nint)buffer;
        public int Height => height;
        public int Width => width;
        public int Size => PixelSize * width * height;

        public Image(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("width/height");

            widthf = this.width = width;
            heightf = this.height = height;

            buffer = (uint*)Marshal.AllocHGlobal(width * height * PixelSize);
        }

        public Image(int width, int height, uint* buffer)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("width/height");
            widthf = this.width = width;
            heightf = this.height = height;
            this.buffer = buffer;
        }

        public static Image FromColor(Color color)
        {
            Image source = new(1, 1);
            source[0, 0] = color;
            return source;
        }

        public static void BufferCopy(Image source, Image destination)
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
            Marshal.FreeHGlobal(Buffer);
        }
    }
}
