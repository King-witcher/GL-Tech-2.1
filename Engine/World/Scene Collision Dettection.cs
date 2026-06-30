
using Engine.Physics;

namespace GLTech.World;

partial class Scene
{
    // ── Raycast pontual (line-of-sight, debug, IA) ──
    public void CastRay(Segment ray, out float distance, out Vector normal)
    {
        RayCastInfo info = new();
        info.displacement.start = ray.start;
        info.displacement.direction = ray.direction;
        var summary = CollisionSystem.GetCollision(info);
        distance = summary.fraction;
        normal = summary.surfaceNormal;
    }

    public RayCastResult RayCast(RayCastInfo info) => CollisionSystem.GetCollision(info);
}
