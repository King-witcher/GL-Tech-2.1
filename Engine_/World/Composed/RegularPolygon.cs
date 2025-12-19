namespace GLTech.World.Composed
{
    public sealed class RegularPolygon : Polygon
    {
        static Logger logger = new Logger(typeof(RegularPolygon).Name);

        public RegularPolygon(Vector position, int vertices, float radius, Texture texture)
        {
            if (vertices <= 2)
                logger.Warn("An actual polygon has at least 3 vertices. If you want to make a polygon with only 2 vertices, consider creating two walls manually.");


            if (radius == 0)
            {
                logger.Error("Radius cannot be zero. The polygon was not be created and the RegularPolygon was returned as an empty RegularPolygon.");

                PositionData = position;
                DirectionData = Vector.Forward;

                return;
            }

            PositionData = position;
            DirectionData = Vector.Forward * radius;

            Build(
                verts: Vector.GetRegularPolygon(position, radius, vertices),
                texture
            );
        }
    }
}
