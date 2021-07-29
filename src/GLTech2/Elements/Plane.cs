namespace GLTech2
{
    /// <summary>
    /// Represents a vertical plane on the map.
    /// </summary>
    public unsafe class Plane : Element
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal SPlane* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        private protected override Vector DirectionData
        {
            get => unmanaged->direction;
            set => unmanaged->direction = value;
        }
        #endregion

        /// <summary>
        /// Gets and sets the starting point of the plane.
        /// </summary>
        public Vector Start
        {
            // Just a bit spaguetti
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the plane.
        /// </summary>
        public Vector End
        {
            // Spaguetti?
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
