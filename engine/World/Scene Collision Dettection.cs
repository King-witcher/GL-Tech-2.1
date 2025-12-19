
using Engine.Physics;

namespace GLTech.World;

partial class Scene
{
    public Collider CastRay(Segment ray, out float distance)
    {
        Collider nearest = null;
        distance = float.PositiveInfinity;

        foreach (Collider current in colliders)
        {
            current.Test(ray, out float currentDistance);

            if (currentDistance < distance)
            {
                nearest = current;
                distance = currentDistance;
            }
        }
        return nearest;
    }

    //public unsafe void CastRay(Segment ray, out float distance, out Vector normal)
    //{
    //    ray.Direction /= ray.Direction.Module;

    //    // Thanks Carmack ;)
    //    float Q_rsqrt(float number)
    //    {
    //        long i;
    //        float x2, y;
    //        const float threehalfs = 1.5f;

    //        x2 = number * 0.5F;
    //        y = number;
    //        i = *(long*)&y;                       // evil floating point bit level hacking
    //        i = 0x5f3759df - (i >> 1);               // what the fuck? 
    //        y = *(float*)&i;
    //        y = y * (threehalfs - (x2 * y * y));   // 1st iteration
    //        y = y * (threehalfs - (x2 * y * y));   // 2nd iteration, this can be removed

    //        return y;
    //    }

    //    RawCollider* nearest = unmanaged->CollisionRaycast(ray, out distance);

    //    if (nearest != null)
    //    {
    //        // Perpendicular to the collision...
    //        normal = new Vector(nearest->direction.Y, -nearest->direction.X);

    //        // ...and with module 1f.
    //        normal *= Q_rsqrt(normal.X * normal.X + normal.Y * normal.Y);
    //    }
    //    else
    //        normal = Vector.Zero;
    //}

    public void CastRay(Segment ray, out float distance, out Vector normal)
    {
        RayCastInfo info = new();
        info.segment.start = ray.start;
        info.segment.direction = ray.direction;
        var summary = colliderSystem.RayCast(info);
        distance = summary.ray_factor;
        normal = summary.normal;
    }

    public CollisionSummary RayCast(RayCastInfo info) => colliderSystem.RayCast(info);
}
