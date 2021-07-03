namespace GLTech2
{
    partial class Element
    {
        /// <summary>
        /// Determines wheter the element should or not handle collisions.
        /// </summary>
        internal bool HandleCollisions { get; set; } = true;

        /// <summary>
        /// Not implemented yet
        /// </summary>
        internal float CollisionRadius { get; set; } = 0f;

        /// <summary>
        /// Moves the object in one direction relatively to it's direction; in other words, the direction of the module vector.
        /// </summary>
        /// <param name="translation">Direction to move</param>
        /// <param name="stopOnCollision">Determines whether the translation should stop or continue after a collision</param>
        public void Translate(Vector translation, bool stopOnCollision = false)
        {
            if (!stopOnCollision)
                Translation += translation;
            else
            {
                Vector worldTranslation = referencePoint == null ?
                    translation :
                    translation.Disprojection(referencePoint.WorldPosition, referencePoint.WorldRotation);

                Scene.RayCast(
                    new Ray(WorldPosition, worldTranslation),
                    out float distance);

                if (distance < worldTranslation.Module)
                    worldTranslation.Module = distance;

                translation = worldTranslation.Projection(referencePoint.WorldPosition, referencePoint.WorldPosition);

                Translation += translation;
            }
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

        private void ClipVelocity()
        {
            if (WorldSpeed == 0f)
                return;

            Vector veldir = worldVelocity / worldVelocity.Module;

            PhysicalPlane plane = Scene.RayCast(new Ray(WorldPosition, veldir), out float distance);
        }
    }
}
