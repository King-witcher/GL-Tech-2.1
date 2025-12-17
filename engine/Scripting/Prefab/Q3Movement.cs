using Engine.Scripting.Physics;
using Engine.Input;
using Engine.World;
using System;

namespace Engine.Scripting.Prefab
{
    public sealed class Q3Movement : Script
    {
        static Logger logger = new(typeof(Q1Movement).Name);
        KinematicBody body;
        float zspeed = 0f;
        bool grounded = true;

        public Q3Movement(KinematicBody body)
        {
            this.body = body;
        }

        public bool AlwaysRun { get; set; } = true;
        public float TurnSpeed { get; set; } = 90f;
        public float JumpSpeed { get; set; } = 2.7f;
        public float Gravity { get; set; } = 8f;
        public float StopSpeed { get; set; } = 1f;
        public float MaxSpeed { get; set; } = 3.2f;
        public float Acceleration { get; set; } = 10f;
        public float AirAcceleration { get; set; } = 1f;
        public float Friction { get; set; } = 6f;
        public float Height { get; set; } = 0.5f;
        public ScanCode StepForward { get; set; } = ScanCode.W;
        public ScanCode StepBack { get; set; } = ScanCode.S;
        public ScanCode StepLeft { get; set; } = ScanCode.A;
        public ScanCode StepRight { get; set; } = ScanCode.D;
        public ScanCode TurnRight { get; set; } = ScanCode.RIGHT;
        public ScanCode TurnLeft { get; set; } = ScanCode.LEFT;
        public ScanCode ChangeRun_Walk { get; set; } = ScanCode.LSHIFT;

        void OnStart()
        {
            if (Entity is Camera camera)
                camera.Z = Height;
            else
                logger.Error($"Entity {Entity} is not a camera.");
        }

        void OnFrame()
        {
            if (Entity is Camera camera)
            {
                DetectJump();
                UpdateVelocity(GetMaxSpeed());
                UpdateZ(camera);

                if (Keyboard.IsKeyDown(ScanCode.LEFT))
                    camera.RelativeRotation -= Time.TimeStep * TurnSpeed;
                if (Keyboard.IsKeyDown(ScanCode.RIGHT))
                    camera.RelativeRotation += Time.TimeStep * TurnSpeed;
            }
        }

        void DetectJump()
        {
            if (Keyboard.IsKeyDown(ScanCode.SPACE) && grounded)
            {
                zspeed = JumpSpeed;
                grounded = false;
            }
        }

        void UpdateZ(Camera camera)
        {
            if (grounded) return;

            camera.Z += zspeed * Time.TimeStep;
            if (camera.Z < Height)
            {
                camera.Z = Height;
                grounded = true;
                zspeed = 0f;
            }

            if (camera.Z > 1f)
            {
                camera.Z = 0.9f;
                zspeed = 0f;
            }

            zspeed -= Gravity * Time.TimeStep;
        }

        float GetMaxSpeed()
        {
            bool run = AlwaysRun;
            if (Keyboard.IsKeyDown(ChangeRun_Walk))
                run = !run;

            if (run)
                return MaxSpeed;
            else
                return MaxSpeed / 2;
        }

        void UpdateVelocity(float maxspeed)
        {
            Vector wishdir = GetWishDir();

            if (grounded)
            {
                ApplyFriction();
                Accelerate(wishdir, maxspeed, Acceleration);
            }
            else
            {
                Accelerate(wishdir, 13f / 16f * maxspeed, AirAcceleration);
            }

            if (body.Speed < .01f) body.Velocity = (0, 0);
        }

        void Accelerate(Vector wishdir, float wishspeed, float accel)
        {
            float currentspeed = Vector.DotProduct(body.Velocity, wishdir);
            float addspeed = wishspeed - currentspeed;
            if (addspeed <= 0) return;
            float accelspeed = accel * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            body.Velocity += accelspeed * wishdir;
        }

        void AirAccelerate(Vector wishdir, float wishspeed)
        {
            float wishspd = wishspeed;
            if (wishspd > .3f)
                wishspd = .3f;

            float currentspeed = Vector.DotProduct(body.Velocity, wishdir);
            float addspeed = wishspd - currentspeed;

            if (addspeed < 0) return;

            float accelspeed = Acceleration * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            body.Velocity += accelspeed * wishdir;
        }

        Vector GetWishDir()
        {
            Vector result = Vector.Zero;

            if (Keyboard.IsKeyDown(StepForward))
                result += Vector.Forward;
            if (Keyboard.IsKeyDown(StepBack))
                result += Vector.Backward;
            if (Keyboard.IsKeyDown(StepLeft))
                result += Vector.Left;
            if (Keyboard.IsKeyDown(StepRight))
                result += Vector.Right;

            result *= Entity.RelativeDirection;

            if (result.Module == 0)
                return Vector.Zero;
            else
                return result / result.Module;
        }


        void ApplyFriction()
        {
            if (body.Speed < 0.01f)
            {
                body.Velocity = (0, 0);
                return;
            }

            float control = body.Speed < StopSpeed ? StopSpeed : body.Speed;
            float drop = control * Friction * Time.TimeStep;

            float newspeed = body.Speed - drop;
            if (newspeed < 0f)
                newspeed = 0f;
            newspeed /= body.Speed;

            body.Velocity *= newspeed;
        }
    }
}
