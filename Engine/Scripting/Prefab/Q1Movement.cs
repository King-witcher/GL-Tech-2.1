using Engine.Scripting.Prefab;
using GLTech.Scripting.Physics;

namespace GLTech.Scripting.Prefab
{
    public sealed class Q1Controller : PlayerController
    {
        public float StopSpeed { get; set; } = 1f;
        public float Acceleration { get; set; } = 10f;
        public float Friction { get; set; } = 6f;
        public float AirSpeedCap { get; set; } = 0.3f;

        protected override void Accelerate(ref Vector velocity, Vector wishVelocity)
        {
            float wishSpeed = wishVelocity.Module;
            if (wishSpeed < float.Epsilon)
                return;

            Vector wishDir = wishVelocity / wishSpeed;

            float currentSpeed = Vector.DotProduct(velocity, wishDir);
            float addSpeed = wishSpeed - currentSpeed;
            if (addSpeed <= 0f)
                return;

            float accelSpeed = Acceleration * wishSpeed * Time.TimeStep;
            if (accelSpeed > addSpeed)
                accelSpeed = addSpeed;

            velocity += accelSpeed * wishDir;
        }

        protected override void AirAccelerate(ref Vector velocity, Vector wishVelocity)
        {
            float wishSpeed = wishVelocity.Module;
            if (wishSpeed < float.Epsilon)
                return;

            Vector wishDir = wishVelocity / wishSpeed;

            float cappedWishSpeed = wishSpeed > AirSpeedCap ? AirSpeedCap : wishSpeed;

            float currentSpeed = Vector.DotProduct(velocity, wishDir);
            float addSpeed = cappedWishSpeed - currentSpeed;
            if (addSpeed <= 0f)
                return;

            float accelSpeed = Acceleration * wishSpeed * Time.TimeStep;
            if (accelSpeed > addSpeed)
                accelSpeed = addSpeed;

            velocity += accelSpeed * wishDir;
        }

        protected override void ApplyFriction(ref Vector velocity)
        {
            float speed = velocity.Module;
            if (speed < 0.01f)
            {
                velocity = Vector.Zero;
                return;
            }

            float control = speed < StopSpeed ? StopSpeed : speed;
            float drop = control * Friction * Time.TimeStep;

            float newSpeed = speed - drop;
            if (newSpeed < 0f)
                newSpeed = 0f;

            velocity *= newSpeed / speed;
        }
    }
}
