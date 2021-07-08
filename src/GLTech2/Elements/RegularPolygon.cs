﻿namespace GLTech2.Elements
{
    /// <summary>
    /// Stores a set of walls that makes a regular polygon.
    /// </summary>
    public sealed class RegularPolygon : Polygon
    {
        private static readonly string className = typeof(RegularPolygon).FullName;

        /// <summary>
        /// Gets a new instance of RegularPolygon.
        /// </summary>
        /// <remarks>
        /// The Element.Position property will be equal to it's center, while it's Element.Normal will point to Vector.Forward and have module equal to the polygon radius.
        /// </remarks>
        /// <param name="position">The center of the polygon</param>
        /// <param name="vertices">How many vertices the polygon has</param>
        /// <param name="radius">The radius of the polygon</param>
        /// <param name="texture">The material of the polygon</param>
        public RegularPolygon(Vector position, int vertices, float radius, Texture texture)
        {
            if (vertices <= 2)
                Debug.InternalLog(
                    origin: className,
                    message: "An actual polygon has at least 3 vertices. If you want to make a polygon with only 2 vertices, consider simply creating a Wall.",
                    debugOption: Debug.Options.Warning);


            if (radius == 0)
            {
                Debug.InternalLog(
                    origin: className,
                    message: "Radius cannot be zero. The polygon was not be created and the RegularPolygon was returned as an empty object.",
                    debugOption: Debug.Options.Error);

                PositionData = position;
                DirectionData = Vector.Forward;

                return;
            }

            PositionData = position;
            DirectionData = Vector.Forward * radius;

            Build(
                Vector.GetRegularPolygon(position, radius, vertices),
                texture);
        }
    }
}