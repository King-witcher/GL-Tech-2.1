
namespace GLTech2.Elements
{
    class Wall : Element
    {
        private protected override Vector PositionData { get; set; }
        private protected override Vector DirectionData { get; set; }
        private Plane visual;
        private Collider physical;

        public Wall(Vector start, Vector end, Texture texture)
        {
            PositionData = start;
            DirectionData = end - start;

            (visual = new Plane(start, end, texture)).ReferencePoint = this;
            (physical = new Collider(start, end)).ReferencePoint = this;
        }

        /// <summary>
        /// Gets and sets the material of the Wall.
        /// </summary>
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
