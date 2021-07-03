namespace GLTech2
{
    /// <summary>
    /// Represents a plane that can be rendered on the screen.
    /// </summary>
    public unsafe class VisualPlane : Element
    {
        internal VisualPlaneData* unmanaged;

        /// <summary>
        /// Gets and sets the starting point of the plane.
        /// </summary>
        public Vector Start
        {
            get => unmanaged->geom_start;
            set => unmanaged->geom_start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the plane.
        /// </summary>
        public Vector End
        {
            get => unmanaged->geom_start + unmanaged->geom_direction;
            set => unmanaged->geom_direction = value - unmanaged->geom_start;
        }

        /// <summary>
        /// Gets and sets the length of the plane.
        /// </summary>
        public float Length
        {
            get => unmanaged->geom_direction.Module;
            set => unmanaged->geom_direction *= value / unmanaged->geom_direction.Module;
        }

        /// <summary>
        /// Gets and sets the material of the plane.
        /// </summary>
        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        public override Vector WorldPosition
        {
            get => Start;
            set => Start = value;
        }

        public override Vector WorldRotation
        {
            get => unmanaged->geom_direction;
            set
            {
                unmanaged->geom_direction = value;
            }
        }

        /// <summary>
        /// Gets a new instance of plane.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="texture">Texture</param>
        public VisualPlane(Vector start, Vector end, Texture texture)
        {
            unmanaged = VisualPlaneData.Create(start, end, texture);
        }

        public override void Dispose()
        {
            VisualPlaneData.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ Start } -- { End }| ";
        }
    }
}
