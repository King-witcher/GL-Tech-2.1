namespace GLTech2
{
    /// <summary>
    /// Represents a plane that can be rendered on the screen.
    /// </summary>
    public unsafe class Plane : Element
    {
        internal SPlane* unmanaged;

        /// <summary>
        /// Gets and sets the starting point of the plane.
        /// </summary>
        public Vector Start
        {
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the plane.
        /// </summary>
        public Vector End
        {
            get => unmanaged->start + unmanaged->direction;
            set => unmanaged->direction = value - unmanaged->start;
        }

        /// <summary>
        /// Gets and sets the length of the plane.
        /// </summary>
        public float Length
        {
            get => unmanaged->direction.Module;
            set => unmanaged->direction *= value / unmanaged->direction.Module;
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
            get => unmanaged->direction;
            set
            {
                unmanaged->direction = value;
            }
        }

        /// <summary>
        /// Gets a new instance of plane.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="texture">Texture</param>
        public Plane(Vector start, Vector end, Texture texture)
        {
            unmanaged = SPlane.Create(start, end, texture);
        }


        public override void Dispose()
        {
            SPlane.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ Start } -- { End }| ";
        }

        internal override void AddToSScene(SScene* data) =>
            data->Add(unmanaged);
    }
}
