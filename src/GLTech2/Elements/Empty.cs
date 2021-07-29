namespace GLTech2
{
    /// <summary>
    /// Represents and empty element that does nothing.
    /// </summary>
    /// <remarks>
    /// This exists basically to allow the user to parent custom groups of elements.
    /// </remarks>
    public sealed class Empty : Element
    {
        /// <summary>
        /// Gets a new instance of Empty.
        /// </summary>
        /// <param name="pos">The position of the empty.</param>
        public Empty(Vector pos)
        {
            PositionData = pos;
            DirectionData = Vector.Forward;
        }

        /// <summary>
        /// Gets a new instance of Empty.
        /// </summary>
        /// <param name="x">The x cordinate</param>
        /// <param name="y">The y cordinate</param>
        public Empty(float x, float y) : this(new Vector(x, y)) { }
    }
}
