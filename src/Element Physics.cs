namespace GLTech2
{
    partial class Element
    {
        public Vector Velocity { get; set; }

        public bool HandleCollisions { get; set; } = true;

        /// <summary>
        /// Not implemented yet
        /// </summary>
        internal float CollisionRadius { get; set; } = 0f;

        /// <summary>
        /// Moves the object in one direction relatively to it's direction; in other words, the direction of the module vector.
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public void Translate(Vector direction, bool stopOnCollision = false)
        {
            Translation += direction;
        }

        /// <summary>
        /// Rotate the object a specified amount.
        /// </summary>
        /// <param name="rotation">angle in degrees</param>
        public void Rotate(float rotation)
        {
            Angle += rotation;
        }

        private void HandleVelocity()
        {

        }
    }
}
