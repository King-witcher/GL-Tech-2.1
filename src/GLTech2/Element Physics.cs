namespace GLTech2
{
    partial class Element
    {

        /// <summary>
        /// Rotate the object a specified amount.
        /// </summary>
        /// <param name="rotation">angle in degrees</param>
        public void Rotate(float rotation)
        {
            RelativeRotation += rotation;
        }
    }
}
