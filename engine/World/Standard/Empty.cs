namespace GLTech.World
{
    public sealed class Empty : Entity
    {
        public Empty(Vector pos)
        {
            PositionData = pos;
            DirectionData = Vector.Forward;
        }

        public Empty(float x, float y) : this(new Vector(x, y)) { }
    }
}
