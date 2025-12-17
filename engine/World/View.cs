
namespace GLTech
{
    public struct View
    {
        internal Vector center;
        internal Vector left;
        internal Vector right;

        float determinant;

        public View(Vector center, Vector left, Vector right)
        {
            this.center = center;
            this.left = left;
            this.right = right;
            determinant = Vector.CrossProduct(this.left, this.right);
        }

        public bool Contains(Vector point)
        {
            Vector shiftedPoint = point - center;

            float leftComponent = Vector.CrossProduct(left, shiftedPoint) / determinant;
            if (leftComponent < 0f)
                return false;

            float rightComponent = Vector.CrossProduct(shiftedPoint, right) / determinant;
            if (rightComponent < 0f)
                return false;

            return true;
        }

        public bool Contains(Segment segment)
        {
            if (Contains(segment.Start) || Contains(segment.Direction + segment.Start))
                return true;

            var rsl = segment.GetRS(new(center, left));
            if (rsl.x < float.PositiveInfinity)
                return true;

            var rsr = segment.GetRS(new(center, right));
            if (rsr.x < float.PositiveInfinity)
                return true;

            return false;
        }

        public static implicit operator View((Vector a, Vector b, Vector c) vectors) =>
            new(vectors.a, vectors.b, vectors.c);
    }
}
