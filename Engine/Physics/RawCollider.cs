using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Physics;

[NativeCppClass]
[StructLayout(layoutKind: LayoutKind.Sequential)]
public unsafe struct RawCollider
{
    public Segment segment;

    public Vector Start
    {
        get => segment.start;
        set => segment.start = value;
    }

    public Vector Direction
    {
        get => segment.direction;
        set => segment.direction = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(Segment ray, out float distance)
    {
        // Culling
        if (segment.direction.x * ray.direction.y - segment.direction.y * ray.direction.x <= 0)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        float
            drx = segment.direction.x,
            dry = segment.direction.y;

        float det = ray.direction.x * dry - ray.direction.y * drx;

        if (det == 0)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        float spldet = ray.direction.x * (ray.start.y - segment.start.y) - ray.direction.y * (ray.start.x - segment.start.x);
        float dstdet = drx * (ray.start.y - segment.start.y) - dry * (ray.start.x - segment.start.x);
        float spltmp = spldet / det;
        float dsttmp = dstdet / det;
        if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
        {
            distance = float.PositiveInfinity;
            return false;
        }
        distance = dsttmp;
        return true;
    }
}
