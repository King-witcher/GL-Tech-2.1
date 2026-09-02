
namespace GLTech.World;

public class Floor : Horizontal
{
    public Floor(Vector start, Vector end, Texture texture) : base(start, end, texture) { }

    public Floor(Vector a, Vector b, Vector c, Texture texture) : base(a, b, c, texture) { }

    /// <summary>
    /// Creates a floor from a convex polygon, triangulated as a fan.
    /// </summary>
    public Floor(Vector[] convexVertices, Texture texture) : base(convexVertices, texture) { }
}
