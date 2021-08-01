
namespace GLTech2.Elements
{
    /// <summary>
    /// Represents a Plane that collides, or a union of a Plane with a Collider.
    /// </summary>
    public class Wall : Element
    {
        private protected override Vector PositionData { get; set; }
        private protected override Vector DirectionData { get; set; }
        private Plane visual;
        private Collider physical;

        /// <summary>
        /// Gets a new instance of Wall.
        /// </summary>
        /// <param name="start">Initial point</param>
        /// <param name="end">Final point</param>
        /// <param name="texture">Texture</param>
        public Wall(Vector start, Vector end, Texture texture)
        {
            PositionData = start;
            DirectionData = end - start;

            (visual = new Plane(start, end, texture)).Parent = this;
            (physical = new Collider(start, end)).Parent = this;
        }

        /// <summary>
        /// Gets and sets the texture of the Wall.
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
