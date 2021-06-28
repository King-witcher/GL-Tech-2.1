
namespace GLTech2.Elements
{
    class Wall : Element
    {
        public override Vector AbsolutePosition { get; set; }
        public override Vector AbsoluteNormal { get; set; }
        private VisualPlane visual;
        private PhysicalPlane physical;

        public Wall(Vector start, Vector end, Texture texture)
        {
            AbsolutePosition = start;
            AbsoluteNormal = end - start;

            (visual = new VisualPlane(start, end, texture)).ReferencePoint = this;
            (physical = new PhysicalPlane(start, end)).ReferencePoint = this;
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
