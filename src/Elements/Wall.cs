using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GLTech2
{
    /// <summary>
    /// Represents a wall that can be rendered on the screen.
    /// </summary>
    public unsafe class Wall : Element
    {
        internal WallData* unmanaged;

        /// <summary>
        /// Gets and sets the starting point of the wall.
        /// </summary>
        public Vector StartPoint
        {
            get => unmanaged->geom_start;
            set => unmanaged->geom_start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the wall.
        /// </summary>
        public Vector EndPoint
        {
            get => unmanaged->geom_start + unmanaged->geom_direction;
            set => unmanaged->geom_direction = value - unmanaged->geom_start;
        }

        /// <summary>
        /// Gets and sets the length of the wall.
        /// </summary>
        public float Length
        {
            get => unmanaged->geom_direction.Module;
            set => unmanaged->geom_direction *= value / unmanaged->geom_direction.Module;
        }

        /// <summary>
        /// Gets and sets the material of the wall.
        /// </summary>
        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        private protected override Vector AbsolutePosition
        {
            get => StartPoint;
            set => StartPoint = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => unmanaged->geom_direction;
            set
            {
                unmanaged->geom_direction = value;
            }
        }

        /// <summary>
        /// Gets a new instance of wall.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="texture">Texture</param>
        public Wall(Vector start, Vector end, Texture texture)
        {
            unmanaged = WallData.Create(start, end, texture);
        }
        
        public Wall(Vector start, float angle_deg, float length, Texture texture)
        {
            unmanaged = WallData.Create(start, angle_deg, length, texture);
        }

        [Obsolete]
        public static Wall[] CreatePolygon(Texture texture, params Vector[] verts) //Beta
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            int total_walls = verts.Length;
            Wall[] result = new Wall[total_walls];

            Texture currentTexture = texture;
            currentTexture.hrepeat /= total_walls;

            for (int i = 0; i < total_walls - 1; i++)
            {
                currentTexture.hoffset = texture.hoffset + texture.hrepeat * i / (total_walls);
                result[i] = new Wall(verts[i], verts[i + 1], currentTexture);
            }

            currentTexture.hoffset = texture.hoffset + texture.hrepeat * (total_walls - 1) / total_walls;
            result[total_walls - 1] = new Wall(verts[total_walls - 1], verts[0], currentTexture);

            return result;
        }

        public override void Dispose()
        {
            WallData.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ StartPoint } -- { EndPoint }| ";
        }
    }
}
