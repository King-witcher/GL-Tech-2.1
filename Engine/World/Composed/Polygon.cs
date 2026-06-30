namespace GLTech.World.Composed
{
    public class Polygon : Entity
    {
        // The reason behind this is SPAGUETTI. I must change as quick as possible.
        private protected override Vector PositionData { get; set; } = Vector.Zero;
        private protected override Vector DirectionData { get; set; } = Vector.North;

        private protected Polygon() { }

        public Polygon(Vector[] vertices, Texture texture, bool collision = true)
        {
            if (vertices == null)
                throw new ArgumentNullException("\"vertices\" cannot be null.");

            Build(vertices, texture, collision);
        }

        // This exists because C# doesn't allow to call base constructor anywhere, so here it is.
        private protected void Build(Vector[] verts, Texture texture, bool collision)
        {
            if (verts.Length == 0)
                return;

            int total_walls = verts.Length;

            Texture currentTexture = texture;
            currentTexture.hrepeat /= total_walls;

            for (int i = 0; i < total_walls - 1; i++)
            {
                currentTexture.hoffset = texture.hoffset + texture.hrepeat * i / (total_walls);
                Entity e;
                if (collision)
                    e = new Wall(
                        start: verts[i],
                        end: verts[i + 1],
                        texture: currentTexture);
                else
                    e = new Plane(
                        start: verts[i],
                        end: verts[i + 1],
                        texture: currentTexture);

                e.Parent = this;
            }

            currentTexture.hoffset = texture.hoffset + texture.hrepeat * (total_walls - 1) / total_walls;

            new Plane(
                start: verts[total_walls - 1],
                end: verts[0],
                texture: currentTexture)
                    .Parent = this;
        }
    }
}
