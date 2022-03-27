using System;

using Engine.World;

namespace Engine.Scripting.Physics
{
    public class PointCollider : KinematicBody
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
            Velocity += velocity;
        }

        public override void AngularAccelerate(float acceleration)
        {
            throw new NotImplementedException();
        }

        void OnFrame()
        {
            if (HandleCollisions)
                ClipCollisions();
            Entity.WorldPosition += Velocity * Frame.DeltaTime;
        }

        // Esse método faz exatamente o que você sugeriu, professor =]
        private void ClipCollisions()
        {
            if (Speed == 0)
                return;

            // Step relative to current frame
            float deltaS = Speed * Frame.DeltaTime;

            Scene.CastRay(
                new Ray(Entity.WorldPosition, Velocity),
                out float cllsn_dist,
                out Vector cllsn_normal);

            // If it is in collision route for the next frame, cap the speed so that 
            if (deltaS > cllsn_dist - 0.01f)
            {
                // Compensate the current step.
                Vector compensation = cllsn_normal * (Vector.DotProduct(Velocity, cllsn_normal));
                Velocity -= compensation;

                // Test against a second collision. If so, stop.
                deltaS = Speed * Frame.DeltaTime;

                Scene.CastRay(
                    new Ray(Entity.WorldPosition, Velocity),
                    out cllsn_dist,
                    out cllsn_normal);

                if (deltaS > cllsn_dist - 0.01f)
                    Velocity = Vector.Zero;
            }
        }
    }
}
