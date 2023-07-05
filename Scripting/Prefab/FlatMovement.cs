﻿using Engine.Scripting.Physics;
using Engine.World;
using Engine.Input;

namespace Engine.Scripting.Prefab
{
    public sealed class FlatMovement : Script
    {
        KinematicBody body;

        public FlatMovement(KinematicBody body)
        {
            this.body = body;
        }

        public bool HandleCollisions { get; set; } = true;

        public bool AlwaysRun { get; set; } = true;

        public float WalkSpeed { get; set; } = 0.75f;

        public float RunSpeed { get; set; } = 2.5f;

        public float TurnSpeed { get; set; } = 90f;

        public ScanCode StepForward { get; set; } = ScanCode.W;

        public ScanCode StepBack { get; set; } = ScanCode.S;

        public ScanCode StepLeft { get; set; } = ScanCode.A;

        public ScanCode StepRight { get; set; } = ScanCode.D;

        public ScanCode TurnRight { get; set; } = ScanCode.RIGHT;

        public ScanCode TurnLeft { get; set; } = ScanCode.LEFT;

        public ScanCode ChangeRun_Walk { get; set; } = ScanCode.LSHIFT;

        void OnFrame()
        {
            // Check speed
            bool run = AlwaysRun;
            if (Keyboard.IsKeyDown(ChangeRun_Walk))
                run = !run;

            float speed;
            if (run)
                speed = RunSpeed;
            else speed = WalkSpeed;


            Vector wishdir = (0, 0);
            if (Keyboard.IsKeyDown(StepForward))
                wishdir += Vector.Forward;
            if (Keyboard.IsKeyDown(StepBack))
                wishdir += Vector.Backward;
            if (Keyboard.IsKeyDown(StepLeft))
                wishdir += Vector.Left;
            if (Keyboard.IsKeyDown(StepRight))
                wishdir += Vector.Right;

            // Suboptimal
            if (wishdir.Module != 0)
                wishdir /= wishdir.Module;

            body.Velocity = wishdir * speed * Entity.WorldDirection;

            // Turn
            if (Keyboard.IsKeyDown(TurnLeft))
                Entity.Rotate(-TurnSpeed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(TurnRight))
                Entity.Rotate(TurnSpeed * Frame.DeltaTime);
        }

        private void TryTranslate(Vector translation)
        {
            // Treats as original
            translation *= Entity.WorldDirection;

            float translMod = translation.Module;
            if (translMod == 0)
                return;

            if (HandleCollisions)
            {
                Scene.CastRay(
                    new Segment(Entity.WorldPosition, translation),
                    out float distance,
                    out Vector normal);

                if (translMod >= distance)
                {
                    Vector excess = (translMod - distance) * (translation / translMod);

                    normal /= normal.Module; // Optimizable by fast inverse square root
                    Vector compensation = normal * (Vector.DotProduct(excess, normal) - 0.01f);
                    translation -= compensation;

                    Scene.CastRay(new Segment(Entity.WorldPosition, translation), out float newDist, out Vector _);

                    if (newDist < translation.Module)
                    {
                        translation = Vector.Zero;
                    }
                }
            }

            Entity.WorldPosition += translation;
        }
    }
}
