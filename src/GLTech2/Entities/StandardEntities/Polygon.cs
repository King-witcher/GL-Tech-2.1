﻿using System;

using GLTech2.Drawing;

namespace GLTech2.Elements
{
    public class Polygon : Entity
    {
        // The reason behind this is SPAGUETTI. I must change as quick as possible.
        private protected override Vector PositionData { get; set; } = Vector.Zero;
        private protected override Vector DirectionData { get; set; } = Vector.Forward;

        private protected Polygon() { }

        public Polygon(Vector[] vertices, Texture texture)
        {
            if (vertices == null)
                throw new ArgumentNullException("\"vertices\" cannot be null.");

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
