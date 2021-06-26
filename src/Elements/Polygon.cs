using System;

namespace GLTech2.Elements
{
    /// <summary>
    /// Represents a set of walls that make up a polygon.
    /// </summary>
    public class Polygon : Element
    {
        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }

        private protected Polygon() { }

        /// <summary>
        /// Gets a new instance of Polygon given its vertices.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon</param>
        /// <param name="texture">The texture that will be stretched on the polygon</param>
        public Polygon(Vector[] vertices, Texture texture)
        {
            if (vertices is null)
                throw new ArgumentNullException("\"vertices\" cannot be null.");

            AbsolutePosition = Vector.Origin;
            AbsoluteNormal = Vector.Forward;

            Build(vertices, texture);
        }

        // This exists because C# doesn't allow to call base constructor anywhere, so here it is.
        private protected void Build(Vector[] verts, Texture texture)
        {
            if (verts.Length == 0)
                return;

            int total_walls = verts.Length;

            Texture currentTexture = texture;
            currentTexture.hrepeat /= total_walls;

            for (int i = 0; i < total_walls - 1; i++)
            {
                currentTexture.hoffset = texture.hoffset + texture.hrepeat * i / (total_walls);

                new Plane(
                    start: verts[i],
                    end: verts[i + 1],
                    texture: currentTexture)
                        .Parent = this;
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
