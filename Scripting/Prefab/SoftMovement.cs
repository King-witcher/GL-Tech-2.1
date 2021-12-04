using Engine.Scripting.Physics;

namespace Engine.Scripting.Prefab
{
    public sealed class SoftMovement : Script
    {
        KinematicBody body;

        public SoftMovement()
        {
            // Spaguetti
            // Sei que isso fere o Princípio da Inversão de Dependência (D do SOLID), mas vou deixar como açúcar sintático.
            // Ficarei de olho caso o nível de complexidade/acoplamento aumente mais do que isso.
            this.body = new NoclipMode();
        }

        public SoftMovement(KinematicBody body)
        {
            this.body = body;
        }

        public bool AlwaysRun { get; set; } = true;
        public float MaxSpeed { get; set; } = 5f;
        public float TurnSpeed { get; set; } = 90f;
        public float Friction { get; set; } = 10f;
        public float Acceleration { get; set; } = 10f;
        public InputKey StepForward { get; set; } = InputKey.W;
        public InputKey StepBack { get; set; } = InputKey.S;
        public InputKey StepLeft { get; set; } = InputKey.A;
        public InputKey StepRight { get; set; } = InputKey.D;
        public InputKey TurnRight { get; set; } = InputKey.Right;
        public InputKey TurnLeft { get; set; } = InputKey.Left;
        public InputKey ChangeRun_Walk { get; set; } = InputKey.ShiftKey;

        void OnFrame()
        {
            UpdateVelocity(GetMaxSpeed());
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
