using Engine.Physics;
using GLTech.World;

namespace GLTech.Scripting.Physics
{
    public class PointCollider : KinematicBody
    {
        private Vector realPosition;
        private Vector framePosition;
        private Vector error = Vector.Zero;
        public bool HandleCollisions { get; set; } = true;
        public Vector StartPosition { get; set; } = Vector.Zero;
        public float MinDistance { get; set; } = 0.1f;

        void Start()
        {
            realPosition = StartPosition;
            framePosition = StartPosition;
            Entity.WorldPosition = StartPosition;
        }

        void OnFixedTick()
        {
            Vector step = Velocity * Time.TimeStep;

            if (HandleCollisions && Speed != 0)
            {
                RayCastInfo info = new();
                info.segment.start = realPosition;
                while (true)
                {
                    info.segment.direction = step;
                    var summary = Scene.RayCast(info);

                    // Simulate ray factor as if it was closer by MinDistance
                    summary.ray_factor += MinDistance / Vector.DotProduct(summary.normal, step);

                    if (summary.ray_factor >= 1f) break;

                    var preHitStep = step * summary.ray_factor;
                    var overStep = step - preHitStep;
                    var slideStep = overStep - summary.normal * Vector.DotProduct(summary.normal, overStep);
                    step = preHitStep + slideStep;
                    Velocity -= summary.normal * Vector.DotProduct(Velocity, summary.normal);

                    info.segment.direction = step;
                    summary = Scene.RayCast(info);
                    summary.ray_factor += MinDistance / Vector.DotProduct(summary.normal, step);
                }
            }

            realPosition += step;
            error = framePosition - realPosition;
        }

        void OnFrame()
        {
            framePosition += Velocity * Time.TimeStep;
            var correction = error * Time.TimeStep / Time.FixedTimestep;
            framePosition -= correction;
            error -= correction;
            Entity.WorldPosition = framePosition;
        }

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

        private void ClipCollisions()
        {
            if (Speed == 0) return;

            // Step relative to current frame
            float deltaS = Speed * Time.TimeStep;

            Scene.CastRay(
                new Segment(realPosition, Velocity),
                out float c_dist,
                out Vector c_normal
            );

            // If it is in collision route for the next frame, cap the speed so that 
            if (deltaS > c_dist - MinDistance)
            {
                // Compensate the current step.
                Vector compensation = c_normal * (Vector.DotProduct(Velocity, c_normal));
                Velocity -= compensation;

                // Test against a second collision. If so, stop.
                deltaS = Speed * Time.TimeStep;

                Scene.CastRay(
                    new Segment(realPosition, Velocity),
                    out c_dist,
                    out c_normal
                );

                if (deltaS > c_dist - MinDistance)
                    Velocity = Vector.Zero;
            }
        }
    }
}
