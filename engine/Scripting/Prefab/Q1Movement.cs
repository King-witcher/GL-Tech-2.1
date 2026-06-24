using Engine.Physics;
using Engine.Scripting.Prefab;

namespace GLTech.Scripting.Prefab
{
    public sealed class Q1Movement : PlayerController
    {
        public float StopSpeed { get; set; } = 1f;
        public float MaxSpeed { get; set; } = 3.2f;
        public float Acceleration { get; set; } = 10f;
        public float AirAcceleration { get; set; } = 10f;
        public float Friction { get; set; } = 6f;

        //void HandleJump()
        //{
        //    if ((Input.WasKeyPressed(Jump) || Input.IsKeyDown(Jump)) && grounded)
        //    {
        //        zspeed = JumpSpeed;
        //        grounded = false;
        //    }
        //}

        //void UpdateZ(ref float z)
        //{
        //    if (grounded) return;

        //    z += zspeed * Time.TimeStep;
        //    if (camera.Z < Height)
        //    {
        //        z = Height;
        //        grounded = true;
        //        zspeed = 0f;
        //    }

        //    if (z > 1f)
        //    {
        //        z = 0.9f;
        //        zspeed = 0f;
        //    }

        //    zspeed -= Gravity * Time.TimeStep;
        //}

        void GroundAccelerate(ref Vector source, Vector wishdir, float wishspeed)
        {
            //ApplyFriction(rb);
            float currentspeed = Vector.DotProduct(source, wishdir);
            float addspeed = wishspeed - currentspeed;
            if (addspeed < 0) return;
            float accelspeed = Acceleration * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            source += accelspeed * wishdir;
        }

        void AirAccelerate(RigidBody rb, Vector wishdir, float wishspeed)
        {
            float wishspd = wishspeed;
            if (wishspd > .3f)
                wishspd = .3f;

            float currentspeed = Vector.DotProduct(rb.Velocity, wishdir);
            float addspeed = wishspd - currentspeed;

            if (addspeed < 0) return;

            float accelspeed = AirAcceleration * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            rb.Velocity += accelspeed * wishdir;
        }

        protected override void Accelerate(ref Vector source, Vector wishvel, bool grounded)
        {
            var wishspeed = wishvel.Module;
            if (wishspeed < float.Epsilon) return;

            if (grounded)
                GroundAccelerate(ref source, wishvel / wishspeed, wishspeed);
            else
                GroundAccelerate(ref source, wishvel / wishspeed, wishspeed);
        }

        protected override void ApplyFriction(ref Vector source)
        {
            var speed = source.Module;
            if (speed < 0.01f)
            {
                source = (0, 0);
                return;
            }

            float control = speed < StopSpeed ? StopSpeed : speed;
            float drop = control * Friction * Time.TimeStep;

            float newspeed = speed - drop;
            if (newspeed < 0f)
                newspeed = 0f;
            newspeed /= speed;

            source *= newspeed;
        }
    }
}
