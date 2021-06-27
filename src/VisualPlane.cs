namespace GLTech2
{
    /// <summary>
    /// Represents a wall that can be rendered on the screen.
    /// </summary>
    public unsafe class VisualPlane : Element
    {
        internal VisualPlaneData* unmanaged;

        /// <summary>
        /// Gets and sets the starting point of the wall.
        /// </summary>
        public Vector StartPoint
        {
            get => unmanaged->geom_start;
            set => unmanaged->geom_start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the wall.
        /// </summary>
        public Vector EndPoint
        {
            get => unmanaged->geom_start + unmanaged->geom_direction;
            set => unmanaged->geom_direction = value - unmanaged->geom_start;
        }

        /// <summary>
        /// Gets and sets the length of the wall.
        /// </summary>
        public float Length
        {
            get => unmanaged->geom_direction.Module;
            set => unmanaged->geom_direction *= value / unmanaged->geom_direction.Module;
        }

        /// <summary>
        /// Gets and sets the material of the wall.
        /// </summary>
        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        private protected override Vector AbsolutePosition
        {
            get => StartPoint;
            set => StartPoint = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => unmanaged->geom_direction;
            set
            {
                unmanaged->geom_direction = value;
            }
        }

        /// <summary>
        /// Gets a new instance of wall.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="texture">Texture</param>
        public VisualPlane(Vector start, Vector end, Texture texture)
        {
            unmanaged = VisualPlaneData.Create(start, end, texture);
        }
        
        public VisualPlane(Vector start, float angle_deg, float length, Texture texture)
        {
            unmanaged = VisualPlaneData.Create(start, angle_deg, length, texture);
        }

        public override void Dispose()
        {
            VisualPlaneData.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ StartPoint } -- { EndPoint }| ";
        }
    }
}
