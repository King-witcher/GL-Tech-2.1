using Struct = GLTech.Structs.TriangleStruct;

namespace GLTech.World
{
    public abstract unsafe class Horizontal : Entity
    {
        // One entity may own several triangles (a quad owns two, a convex polygon
        // owns a fan); they move and are retextured together.
        internal Struct*[] triangles;

        private protected override Vector PositionData
        {
            get => triangles[0]->a;
            set
            {
                Vector delta = value - triangles[0]->a;
                foreach (Struct* triangle in triangles)
                    triangle->Translate(delta);
            }
        }

        private protected override Vector DirectionData
        {
            get => Vector.Zero;
            set { }
        }

        public Vector Position
        {
            get => PositionData;
            set => PositionData = value;
        }

        public Texture Texture
        {
            get => triangles[0]->texture;
            set
            {
                foreach (Struct* triangle in triangles)
                    triangle->texture = value;
            }
        }

        internal Horizontal(Vector a, Vector b, Vector c, Texture texture)
        {
            triangles = new[] { Struct.Create(a, b, c, texture) };
        }

        internal Horizontal(Vector start, Vector end, Texture texture)
        {
            var bottom = MathF.Min(start.y, end.y);
            var top = MathF.Max(start.y, end.y);
            var left = MathF.Min(start.x, end.x);
            var right = MathF.Max(start.x, end.x);

            triangles = new[]
            {
                Struct.Create((left, top), (right, top), (left, bottom), texture),
                Struct.Create((right, top), (right, bottom), (left, bottom), texture),
            };
        }

        // Convex polygon, triangulated as a fan around the first vertex.
        internal Horizontal(Vector[] vertices, Texture texture)
        {
            if (vertices is null || vertices.Length < 3)
                throw new ArgumentException("A polygon requires at least 3 vertices.", nameof(vertices));

            triangles = new Struct*[vertices.Length - 2];
            for (int i = 0; i < triangles.Length; i++)
                triangles[i] = Struct.Create(vertices[0], vertices[i + 1], vertices[i + 2], texture);
        }

        public override void Dispose()
        {
            if (triangles is null)
                return;

            foreach (Struct* triangle in triangles)
                Struct.Delete(triangle);
            triangles = null!;
        }

        public override string ToString()
        {
            return $"<{GetType().Name}: {Position}>";
        }
    }
}
