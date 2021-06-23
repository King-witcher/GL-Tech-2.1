using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabElements
{
    /// <summary>
    /// Provides an easier way to create regular polygons of walls.
    /// </summary>
    public sealed class RegularPolygon : Element
    {
        /// <summary>
        /// Gets a new instance of RegularPolygon
        /// </summary>
        /// <param name="position">The center of the polygon</param>
        /// <param name="edges">How many edges the polygon has</param>
        /// <param name="radius">The radius of the polygon</param>
        /// <param name="material">The material of the polygon</param>
        public RegularPolygon(Vector position, int edges, float radius, Texture material)
        {
            if (edges <= 2)
                throw new ArgumentException("\"edges\" must be greater than 2.");
            if (radius == 0)
                throw new ArgumentException("\"radius\" cannot be zero.");

            AbsolutePosition = position;
            AbsoluteNormal = Vector.Forward;

            Vector[] verts = Vector.GetPolygon(position, radius, edges);
            Wall[] walls = Wall.CreatePolygon(material, verts);
            foreach (Wall wall in walls)
                wall.Parent = this;
        }

        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }
    }
}
