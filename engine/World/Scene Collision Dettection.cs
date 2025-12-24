
using Engine.Physics;

namespace GLTech.World;

partial class Scene
{
    public void CastRay(Segment ray, out float distance, out Vector normal)
    {
        RayCastInfo info = new();
        info.segment.start = ray.start;
        info.segment.direction = ray.direction;
        var summary = CollisionSystem.GetCollision(info);
        distance = summary.ray_factor;
        normal = summary.normal;
    }

    public CollisionSummary RayCast(RayCastInfo info) => CollisionSystem.GetCollision(info);
}
