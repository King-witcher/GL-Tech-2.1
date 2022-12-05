using Engine.Imaging;

namespace Engine.World.Composed
{
    public class Wall : Entity
    {
        private protected override Vector PositionData { get; set; }
        private protected override Vector DirectionData { get; set; }
        private Plane visual;
        private Collider physical;

        public Wall(Vector start, Vector end, Texture texture)
        {
            PositionData = start;
            DirectionData = end - start;

            (visual = new Plane(start, end, texture)).Parent = this;
            (physical = new Collider(start, end)).Parent = this;
        }

        public unsafe Texture Texture
        {
            get => visual.unmanaged->texture;
            set
            {
                visual.unmanaged->texture = value;
            }
        }
    }
}
