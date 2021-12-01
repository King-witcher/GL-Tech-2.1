using GLTech2.Imaging;

namespace GLTech2.Entities.StandardEntites
{
    public sealed class RegularPolygon : Polygon
    {
        private static readonly string className = typeof(RegularPolygon).FullName;

        public RegularPolygon(Vector position, int vertices, float radius, Texture texture)
        {
            if (vertices <= 2)
                Debug.InternalLog(
                    message: "An actual polygon has at least 3 vertices. If you want to make a polygon with only 2 vertices, consider creating two walls manually.",
                    debugOption: Debug.Options.Warning);


            if (radius == 0)
            {
                Debug.InternalLog(
                    message: "Radius cannot be zero. The polygon was not be created and the RegularPolygon was returned as an empty RegularPolygon.",
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
