using System;

namespace GLTech2.Behaviours
{
    class PointCollider : KinematicBody
    {
        public bool HandleCollisions { get; set; } = true;

        public override void Accelerate(Vector direction)
        {
            Velocity += direction * Frame.DeltaTime;
        }

        public override void AddAngularVelocity(float momentum)
        {
            throw new NotImplementedException();
        }

        public override void AddVelocity(Vector velocity)
        {
            throw new NotImplementedException();
        }

        public override void AngularAccelerate(float acceleration)
        {
            throw new NotImplementedException();
        }

        void Update()
        {
            if (HandleCollisions)
                ClipCollisions();
            Element.WorldPosition += Velocity * Frame.Time;
        }

        private void ClipCollisions()
        {
            float speed = Speed;
            if (speed == 0f)
                return;

            if (HandleCollisions)
            {
                Scene.RayCast(
                    new Ray(element.WorldPosition, Velocity),
                    out float distance,
                    out Vector normal);

                if (speed >= distance)
                {
                    Vector excess = (speed - distance) * (Velocity / speed);

                    normal /= normal.Module; // Optimizable by fast inverse square root
                    Vector compensation = normal * (Vector.DotProduct(excess, normal) - 0.01f);
                    Velocity -= compensation;

                    Scene.RayCast(new Ray(element.WorldPosition, Velocity), out float newDist, out Vector _);

                    if (newDist < Speed)
                        Velocity = Vector.Zero;
                }
            }
        }
    }
}
