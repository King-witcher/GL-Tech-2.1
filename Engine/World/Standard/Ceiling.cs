
namespace GLTech.World;

public class Ceiling : Horizontal
{
    public Ceiling(Vector start, Vector end, Texture texture) : base(start, end, texture) { }

    public Ceiling(Vector a, Vector b, Vector c, Texture texture) : base(a, b, c, texture) { }

    /// <summary>
    /// Creates a ceiling from a convex polygon, triangulated as a fan.
    /// </summary>
    public Ceiling(Vector[] convexVertices, Texture texture) : base(convexVertices, texture) { }
}
