namespace GLTech2
{
    partial class Element
    {
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
                Vector worldTranslation = parent == null ?
                    translation :
                    translation.Disprojection(parent.PositionData, parent.DirectionData);

                Scene.RayCast(
                    new Ray(PositionData, worldTranslation),
                    out float distance);

                if (distance < worldTranslation.Module)
                    worldTranslation.Module = distance;

                translation = worldTranslation.Projection(parent.PositionData, parent.PositionData);

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
    }
}
