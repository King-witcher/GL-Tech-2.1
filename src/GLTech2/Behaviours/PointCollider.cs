using System;

namespace GLTech2.Behaviours
{
    /// <summary>
    /// An initial implementation for KinematicBody that allows an object to respect collision behaviors.
    /// </summary>
    class PointCollider : KinematicBody
    {
        /// <summary>
        /// Determines whether the object should or not respect collisions.
        /// </summary>
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

        void OnFrame()
        {
            if (HandleCollisions)
                ClipCollisions();
            Element.WorldPosition += Velocity * Frame.DeltaTime;
        }

        // Esse método faz exatamente o que você sugeriu, professor =]
        private void ClipCollisions()
        {
            if (Speed == 0)
                return;

            // Step relative to current frame
            float deltaS = Speed * Frame.DeltaTime;

            Scene.RayCast(
                new Ray(element.WorldPosition, Velocity),
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

                Scene.RayCast(
                    new Ray(element.WorldPosition, Velocity),
                    out cllsn_dist,
                    out cllsn_normal);

                if (deltaS > cllsn_dist - 0.01f)
                    Velocity = Vector.Zero;
            }
        }
    }
}
