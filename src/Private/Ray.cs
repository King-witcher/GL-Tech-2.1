namespace GLTech2
{
    // Spaguetti!
    internal unsafe struct Ray
    {
        internal readonly Vector start;
        internal readonly Vector direction;

        public Ray(Vector start, float angle)
        {
            this.start = start;
            direction = new Vector(angle);
        }
    }
}
