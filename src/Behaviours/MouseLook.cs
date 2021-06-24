
namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Allows the user to move the camera horizontally using the mouse.
    /// </summary>
    public sealed class MouseLook : Behaviour
    {
        /// <summary>
        /// Determines whether the script should or not work.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The sensitivity of the mouse input (similar to Quake)
        /// </summary>
        public float Sensitivity { get; set; } = 5f;

        /// <summary>
        /// Gets a new instance of MouseLook.
        /// </summary>
        public MouseLook() { }

        /// <summary>
        /// Gets a new instance of MouseLook.
        /// </summary>
        /// <param name="sensitivity">Mouse sensitivity</param>
        public MouseLook(float sensitivity)
        {
            this.Sensitivity = sensitivity;
        }

        void Update()
        {
            if (Enabled)
                element.Rotate(Mouse.HShift * 0.022f * Sensitivity);
        }
    }
}
