using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Allows the user to move the camera around the map using keyboard input.
    /// </summary>
    public sealed class FlatMovement : Behaviour
    {
        /// <summary>
        /// Defines whether the camera moves, by default, in the run or walk speed.
        /// </summary>
        public bool AlwaysRun { get; set; } = true;

        /// <summary>
        /// The camera slower speed
        /// </summary>
        public float WalkSpeed { get; set; } = 0.75f;

        /// <summary>
        /// The camera faster speed
        /// </summary>
        public float RunSpeed { get; set; } = 2.5f;

        /// <summary>
        /// The angular speed which the camera turns in degrees per second
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

            // Step
            if (Keyboard.IsKeyDown(StepForward))
                Element.Translate(Vector.Forward * speed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(StepBack))
                Element.Translate(Vector.Backward * speed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(StepLeft))
                Element.Translate(Vector.Left * speed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(StepRight))
                Element.Translate(Vector.Right * speed * Frame.DeltaTime);

            // Turn
            if (Keyboard.IsKeyDown(TurnLeft))
                Element.Rotate(-TurnSpeed * Frame.DeltaTime);
            if (Keyboard.IsKeyDown(TurnRight))
                Element.Rotate(TurnSpeed * Frame.DeltaTime);
        }
    }
}
