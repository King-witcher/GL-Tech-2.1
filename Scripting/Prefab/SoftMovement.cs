using Engine.Scripting.Physics;
using Engine.Input;

namespace Engine.Scripting.Prefab
{
    public sealed class SoftMovement : Script
    {
        KinematicBody body;

        public SoftMovement(KinematicBody body)
        {
            this.body = body;
        }

        public bool AlwaysRun { get; set; } = true;
        public float MaxSpeed { get; set; } = 5f;
        public float TurnSpeed { get; set; } = 90f;
        public float Friction { get; set; } = 10f;
        public float Acceleration { get; set; } = 10f;
        public ScanCode StepForward { get; set; } = ScanCode.W;
        public ScanCode StepBack { get; set; } = ScanCode.S;
        public ScanCode StepLeft { get; set; } = ScanCode.A;
        public ScanCode StepRight { get; set; } = ScanCode.D;
        public ScanCode TurnRight { get; set; } = ScanCode.RIGHT;
        public ScanCode TurnLeft { get; set; } = ScanCode.LEFT;
        public ScanCode ChangeRun_Walk { get; set; } = ScanCode.LSHIFT;

        void OnFrame()
        {
            UpdateVelocity(GetMaxSpeed());

            if (Keyboard.IsKeyDown(ScanCode.LEFT))
                Entity.RelativeRotation -= Frame.DeltaTime * TurnSpeed;
            if (Keyboard.IsKeyDown(ScanCode.RIGHT))
                Entity.RelativeRotation += Frame.DeltaTime * TurnSpeed;
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
            ApplyFriction();
            Vector wishdir = GetWishDir();

            float currentspeed = Vector.DotProduct(body.Velocity, wishdir);
            float addspeed = Acceleration * Frame.DeltaTime * maxspeed;
            if (addspeed < 0)
                return;
            else if (addspeed > maxspeed - currentspeed)
                addspeed = maxspeed - currentspeed;

            body.Velocity += addspeed * wishdir;

            if (body.Speed < .01f) body.Velocity = (0, 0);
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
            body.Velocity -= body.Velocity * Frame.DeltaTime * Friction;
        }
    }
}
