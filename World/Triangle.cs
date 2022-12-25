
namespace Engine
{

    public struct Triangle
    {
        Vector a;
        Vector ab;
        Vector ac;

        float determinant;

        public Triangle(Vector a, Vector b, Vector c)
        {
            this.a = a;
            ab = b - a;
            ac = c - a;
            determinant = Vector.CrossProduct(ab, ac);
        }

        public bool Contains(Vector point)
        {
            Vector shiftedPoint = point - a;

            float factor1 = Vector.CrossProduct(ab, shiftedPoint) / determinant;
            if (factor1 < 0f)
                return false;

            float factor2 = Vector.CrossProduct(shiftedPoint, ac) / determinant;
            if (factor2 < 0f || (factor1 + factor2) > 1)
                return false;

            return true;
        }

        public static implicit operator Triangle ((Vector a, Vector b, Vector c) vectors) =>
            new (vectors.a, vectors.b, vectors.c);
    }
}
