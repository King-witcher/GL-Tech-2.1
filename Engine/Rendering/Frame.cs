using GLTech;

namespace Engine.Rendering
{
    internal unsafe readonly struct ColumnSpans
    {
        public readonly int* start;
        public readonly int* end;

        public ColumnSpans(int* start, int* end)
        {
            this.start = start;
            this.end = end;
        }
    }

    internal unsafe readonly struct WallsInfo
    {
        public readonly int count;
        public readonly Segment* segments;
        public readonly Texture* textures;

        public WallsInfo(int count, Segment* segments, Texture* textures)
        {
            this.count = count;
            this.segments = segments;
            this.textures = textures;
        }
    }

    internal readonly struct CameraInfo
    {
        public readonly float z;
        public readonly Vector position;
        public readonly Vector direction;

        public CameraInfo(Vector position, Vector direction, float z)
        {
            this.position = position;
            this.direction = direction;
            this.z = z;
        }
    }

    internal readonly struct Projection
    {
        public readonly float colHeight1; // col slope
        public readonly float halfWidth;
        public readonly float horizon;
        public readonly float columnSlope;
        public readonly float frustrumSlope; // tan(hFov / 2)

        private Projection(float colHeight1, float halfWidth, float horizon, float columnSlope, float frustrumSlope)
        {
            this.colHeight1 = colHeight1;
            this.halfWidth = halfWidth;
            this.horizon = horizon;
            this.columnSlope = columnSlope;
            this.frustrumSlope = frustrumSlope;
        }

        public static Projection From(in SwapchainImage image, float hFovDeg) =>
            From(image.widthf, image.heightf, hFovDeg);

        public static Projection From(float widthf, float heightf, float hFovDeg)
        {
            const float DEG_TO_RAD = MathF.PI / 180f;

            float frustrumSlope = MathF.Tan(DEG_TO_RAD * hFovDeg * 0.5f);

            float columnlope = 2f * frustrumSlope / widthf;

            return new Projection(
                colHeight1: 1f / columnlope,
                halfWidth: widthf * 0.5f,
                // Rows are sampled at their index, so the screen centre sits between the
                // first and the last one. Halving heightf instead would offset every pass
                // by half a pixel -- and, for an even height, put a row exactly on the
                // horizon, where the floor and ceiling passes divide by it.
                horizon: (heightf - 1f) * 0.5f,
                columnSlope: columnlope,
                frustrumSlope: frustrumSlope);
        }
    }

    internal readonly struct Frame
    {
        public readonly CameraInfo camera;
        public readonly Projection projection;
        public readonly ColumnSpans spans;
        public readonly SwapchainImage image;
        public readonly WallsInfo walls;

        public Frame(
            CameraInfo camera,
            Projection projection,
            ColumnSpans spans,
            SwapchainImage image,
            WallsInfo walls)
        {
            this.camera = camera;
            this.projection = projection;
            this.spans = spans;
            this.image = image;
            this.walls = walls;
        }
    }
}
