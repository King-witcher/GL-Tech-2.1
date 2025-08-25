using System;

using Engine.World;

namespace Engine.Scripting.Physics
{
    public class PointCollider : KinematicBody
    {
        const float MIN_DIST = 0.01f;

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
            Velocity += velocity;
        }

        public override void AngularAccelerate(float acceleration)
        {
            throw new NotImplementedException();
        }

        // FIXME
        void OnFrame()
        {
            if (HandleCollisions)
                ClipCollisions();
            Entity.WorldPosition += Velocity * Frame.DeltaTime;
        }

        private void ClipCollisions()
        {
            if (Speed == 0) return;

            // Step relative to current frame
            float deltaS = Speed * Frame.DeltaTime;

            Scene.CastRay(
                new Segment(Entity.WorldPosition, Velocity),
                out float c_dist,
                out Vector c_normal
            );

            // If it is in collision route for the next frame, cap the speed so that 
            if (deltaS > c_dist - MIN_DIST)
            {
                // Compensate the current step.
                Vector compensation = c_normal * (Vector.DotProduct(Velocity, c_normal));
                Velocity -= compensation;

                // Test against a second collision. If so, stop.
                deltaS = Speed * Frame.DeltaTime;

                Scene.CastRay(
                    new Segment(Entity.WorldPosition, Velocity),
                    out c_dist,
                    out c_normal
                );

                if (deltaS > c_dist - MIN_DIST)
                    Velocity = Vector.Zero;
            }
        }
    }
}
