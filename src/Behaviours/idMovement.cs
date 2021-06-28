namespace GLTech2.Behaviours
{
    /// <summary>
    /// Allows the user to move the camera around the map using keyboard input in a quake-like way. May not work as expected yet.
    /// </summary>
    internal sealed class idMovement : Behaviour
    {
        public bool AlwaysRun { get; set; } = true;
        public float MaxSpeed { get; set; } = 1f;
        public bool TreatCollisions { get; set; } = true;
        public float TurnSpeed { get; set; } = 90f;
        public float Friction { get; set; } = 10f;
        public float Acceleration { get; set; } = 50f;
        public Key StepForward { get; set; } = Key.W;
        public Key StepBack { get; set; } = Key.S;
        public Key StepLeft { get; set; } = Key.A;
        public Key StepRight { get; set; } = Key.D;
        public Key TurnRight { get; set; } = Key.Right;
        public Key TurnLeft { get; set; } = Key.Left;
        public Key ChangeRun_Walk { get; set; } = Key.ShiftKey;

        private float StopSpeed = 1f;

        // Relative to the space.
        Vector velocity;

        void Start()
        {
            velocity = Vector.Origin;
        }

        void OnFrame()
        {
            UpdateVelocity(GetMaxSpeed());
            UpdatePosition();
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

            float currentspeed = Vector.DotProduct(velocity, wishdir);
            float addspeed = Acceleration * Frame.DeltaTime;
            if (addspeed < 0)
                return;
            else if (addspeed > MaxSpeed - currentspeed)
                addspeed = MaxSpeed - currentspeed;

            velocity += addspeed * wishdir;
            if (velocity.Module < 1f)
                velocity.Module = 0;
        }

        void UpdatePosition()
        {
            TryTranslate(velocity * Frame.DeltaTime);
        }

        Vector GetWishDir()
        {
            Vector result = Vector.Origin;

            if (Keyboard.IsKeyDown(StepForward))
                result += Vector.Forward;
            if (Keyboard.IsKeyDown(StepBack))
                result += Vector.Backward;
            if (Keyboard.IsKeyDown(StepLeft))
                result += Vector.Left;
            if (Keyboard.IsKeyDown(StepRight))
                result += Vector.Right;

            result *= Element.Normal;

            if (result.Module == 0)
                return Vector.Origin;
            else
                return result / result.Module;
        }


        void ApplyFriction()
        {
            velocity -= velocity * Frame.DeltaTime * Friction;
        }

        private void TryTranslate(Vector translation)
        {
            if (!TreatCollisions)
                element.Translate(translation);
            else
            {
                if (translation.Module == 0)
                    return;
                float wishdist = translation.Module;
                Vector wishdir = translation * element.Normal;
                wishdir /= wishdir.Module;

                PhysicalPlane plane = Scene.RayCast(new Ray(element.Position, wishdir), out float distance);
                if (plane != null && wishdist > distance)
                {
                    Vector planeVersor = plane.AbsoluteNormal / plane.AbsoluteNormal.Module;
                    Vector wishdir_proj = (wishdir.x * planeVersor.x + wishdir.y * planeVersor.y) * planeVersor;
                    Vector wishdir_ortogonal = wishdir - wishdir_proj;

                    wishdist *= wishdir_proj.Module;
                    wishdir = wishdir_proj / wishdir_proj.Module;

                    // Test
                    Scene.RayCast(new Ray(element.Position, wishdir), out float newDist);

                    if (newDist < wishdist)
                        wishdist = 0f;
                }

                Element.Position += wishdir * wishdist;
            }
        }
    }
}
