using Engine.World;
using System;

namespace Engine.Scripting.Physics
{
    public class PointCollider : KinematicBody
    {
        const float MIN_DIST = 0.01f;

        private Vector lastTruePosition;
        private Vector predictedPosition;
        private Vector smoothPosition;

        public bool HandleCollisions { get; set; } = true;
        public Vector StartPosition { get; set; } = Vector.Zero;

        public override void Accelerate(Vector direction)
        {
            Velocity += direction * Time.TimeStep;
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

        void OnStart()
        {
            lastTruePosition = StartPosition;
            predictedPosition = StartPosition;
            smoothPosition = StartPosition;
        }

        // FIXME
        void OnFixedTick()
        {
            if (HandleCollisions)
                ClipCollisions();
            lastTruePosition += Velocity * Time.TimeStep;
            predictedPosition = lastTruePosition;
        }

        void OnFrame()
        {
            const float SMOOTH_FACTOR = 0.2f;
            predictedPosition += Velocity * Time.TimeStep;
            smoothPosition = predictedPosition * SMOOTH_FACTOR + smoothPosition * (1 - SMOOTH_FACTOR);
            Entity.WorldPosition = smoothPosition;
        }

        private void ClipCollisions()
        {
            if (Speed == 0) return;

            // Step relative to current frame
            float deltaS = Speed * Time.TimeStep;

            Scene.CastRay(
                new Segment(lastTruePosition, Velocity),
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
                deltaS = Speed * Time.TimeStep;

                Scene.CastRay(
                    new Segment(lastTruePosition, Velocity),
                    out c_dist,
                    out c_normal
                );

                if (deltaS > c_dist - MIN_DIST)
                    Velocity = Vector.Zero;
            }
        }
    }
}
