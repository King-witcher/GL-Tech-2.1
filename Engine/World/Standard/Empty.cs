namespace GLTech.World
{
    public sealed class Empty : Entity
    {
        public Empty(Vector pos)
        {
            PositionData = pos;
            DirectionData = Vector.North;
        }

        public Empty(float x, float y) : this(new Vector(x, y)) { }
    }
}
