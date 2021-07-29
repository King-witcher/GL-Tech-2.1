
namespace GLTech2.Behaviours
{
    /// <summary>
    /// Rotate an object based on mouse movement.
    /// </summary>
    /// <remarks>
    /// Allows the user to move the camera using the mouse.
    /// </remarks>
    public sealed class MouseRotation : Behaviour
    {
        /// <summary>
        /// Determines whether this script should or not run.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The mouse sensitivity, with 0.022 yaw (similar to id Tech games)
        /// </summary>
        public float Sensitivity { get; set; } = 5f;

        /// <summary>
        /// Gets a new instance of MouseLook.
        /// </summary>
        public MouseRotation() { }

        /// <summary>
        /// Gets a new instance of MouseLook.
        /// </summary>
        /// <param name="sensitivity">The mouse sensitivity</param>
        public MouseRotation(float sensitivity)
        {
            Sensitivity = sensitivity;
        }

        void OnFrame()
        {
            if (Enabled)
                element.Rotate(Mouse.HShift * 0.022f * Sensitivity);
        }
    }
}
