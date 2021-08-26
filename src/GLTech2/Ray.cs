namespace GLTech2
{
    /// <summary>
    /// Represents a ray.
    /// </summary>
    public unsafe struct Ray
    {
        internal readonly Vector start;
        internal readonly Vector direction;

        /// <summary>
        /// The only initial/final point of the ray.
        /// </summary>
        public Vector Start => start;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Vector Direction => direction;

        /// <summary>
        /// Gets a new instance of ray.
        /// </summary>
        /// <param name="start">Initial point</param>
        /// <param name="direction">Direction</param>
        public Ray(Vector start, Vector direction)
        {
            this.start = start;
            this.direction = direction;
        }

        /// <summary>
        /// Gets a new instance of ray.
        /// </summary>
        /// <param name="start">Initial point</param>
        /// <param name="angle">Angle</param>
        public Ray(Vector start, float angle)
        {
            this.start = start;
            direction = new Vector(angle);
        }
    }
}
