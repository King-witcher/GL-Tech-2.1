﻿#pragma warning disable IDE0051

namespace GLTech2.Behaviours
{
    /// <summary>
    /// A basic test script that makes the object move indefinitely in one direction.
    /// </summary>
    public sealed class Move : Behaviour
    {
        /// <summary>
        /// The direction the object moves
        /// </summary>
        public Vector Direction { get; set; } = Vector.Forward;

        /// <summary>
        /// The speed the object moves
        /// </summary>
        public float Speed { get; set; } = 0.5f;

        void OnFrame()
        {
            Element.Translate(Direction * Speed * Frame.DeltaTime);
        }
    }
}
