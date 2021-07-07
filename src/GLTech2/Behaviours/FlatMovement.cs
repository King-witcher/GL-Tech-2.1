namespace GLTech2.Behaviours
{
    /// <summary>
    /// Allows the user to move the camera around the map using keyboard input.
    /// </summary>
    public sealed class FlatMovement : Behaviour
    {
        KinematicBody body;

        public FlatMovement()
        {
            body = new PointCollider();
        }

        public FlatMovement(KinematicBody body)
        {
            this.body = body;
        }

        /// <summary>
        /// Defines whether the script should or not treat collisions while moving.
        /// </summary>
        public bool HandleCollisions { get; set; } = true;

        /// <summary>
        /// Defines whether the element moves, by default, in the run or walk speed.
        /// </summary>
        public bool AlwaysRun { get; set; } = true;

        /// <summary>
        /// The element slower speed
        /// </summary>
        public float WalkSpeed { get; set; } = 0.75f;

        /// <summary>
        /// The element faster speed
        /// </summary>
        public float RunSpeed { get; set; } = 2.5f;

        /// <summary>
        /// The angular speed which the element turns in degrees per second
        /// </summary>
        public float TurnSpeed { get; set; } = 90f;

        /// <summary>
        /// The key bound to forward movement
        /// </summary>
        public Key StepForward { get; set; } = Key.W;

        /// <summary>
        /// The key bound to backward movement
        /// </summary>
        public Key StepBack { get; set; } = Key.S;

        /// <summary>
        /// The key bound to left movement
        /// </summary>
        public Key StepLeft { get; set; } = Key.A;

        /// <summary>
        /// The key bound to right movement
        /// </summary>
        public Key StepRight { get; set; } = Key.D;

        /// <summary>
        /// The key bound to turn right
        /// </summary>
        public Key TurnRight { get; set; } = Key.Right;

        /// <summary>
        /// The key bound to turn left movement
        /// </summary>
        public Key TurnLeft { get; set; } = Key.Left;

        /// <summary>
        /// The key that changes between walk and run speed
        /// </summary>
        public Key ChangeRun_Walk { get; set; } = Key.ShiftKey;

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

            body.Velocity = wishdir * speed * element.WorldDirection;

            // Turn
            if (Keyboard.IsKeyDown(TurnLeft))
                Element.Rotate(-TurnSpeed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(TurnRight))
                Element.Rotate(TurnSpeed * Frame.DeltaTime);
        }

        private void TryTranslate2(Vector translation)
        {
            if (!HandleCollisions)
                element.Translate(translation * element.Rotation);
            else
            {
                if (translation.Module == 0)
                    return;
                float wishdist = translation.Module;
                Vector wishdir = translation * element.Rotation;
                wishdir /= wishdir.Module;

                Collider plane = Scene.RayCast(new Ray(element.Translation, wishdir), out float distance);
                if (plane != null && wishdist > distance)
                {
                    Vector planeVersor = plane.WorldDirection / plane.WorldDirection.Module;
                    Vector wishdir_proj = (wishdir.x * planeVersor.x + wishdir.y * planeVersor.y) * planeVersor;
                    Vector wishdir_ortogonal = wishdir - wishdir_proj;

                    wishdist *= wishdir_proj.Module;
                    wishdir = wishdir_proj / wishdir_proj.Module;

                    // Test
                    Scene.RayCast(new Ray(element.Translation, wishdir), out float newDist);

                    if (newDist < wishdist)
                        wishdist = 0f;
                }

                Element.Translation += wishdir * wishdist;
            }
        }

        private void TryTranslate(Vector translation)
        {
            // Treats as original
            translation *= element.WorldDirection;

            float translMod = translation.Module;
            if (translMod == 0)
                return;

            if (HandleCollisions)
            {
                Scene.RayCast(
                    new Ray(element.WorldPosition, translation),
                    out float distance,
                    out Vector normal);

                if (translMod >= distance)
                {
                    Vector excess = (translMod - distance) * (translation / translMod);

                    normal /= normal.Module; // Optimizable by fast inverse square root
                    Vector compensation = normal * (Vector.DotProduct(excess, normal) - 0.01f);
                    translation -= compensation;

                    Scene.RayCast(new Ray(element.WorldPosition, translation), out float newDist, out Vector _);

                    if (newDist < translation.Module)
                    {
                        translation = Vector.Zero;
                    }
                }
            }

            element.Translate(translation);
        }
    }
}
