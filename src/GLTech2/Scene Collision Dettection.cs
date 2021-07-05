
namespace GLTech2
{
    partial class Scene
    {
        // Provisional; must be optimized
        public Collider RayCast(Ray ray, out float distance)
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

        // Faster, doesn't depend on managed resources.
        public unsafe void RayCast(Ray ray, out float distance, out Vector normal)
        {
            ray.direction /= ray.direction.Module;
            // ;)
            float Q_rsqrt(float number)
            {
                long i;
                float x2, y;
                const float threehalfs = 1.5f;

                x2 = number * 0.5F;
                y = number;
                i = *(long*)&y;                       // evil floating point bit level hacking
                i = 0x5f3759df - (i >> 1);               // what the fuck? 
                y = *(float*)&i;
                y = y * (threehalfs - (x2 * y * y));   // 1st iteration
                //	y  = y * ( threehalfs - ( x2 * y * y ) );   // 2nd iteration, this can be removed

                return y;
            }

            SCollider* nearest = unmanaged->CollisionTest(ray, out distance);


            if (nearest != null)
            {
                normal = new Vector(nearest->direction.y, -nearest->direction.x);
            }
            else
                normal = Vector.Origin;
        }
    }
}
