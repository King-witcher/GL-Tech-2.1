namespace GLTech2.Behaviours
{
    /// <summary>
    /// Debugs all relative components of an object.
    /// </summary>
    public class DebugComponents : Behaviour
    {
        /// <summary>
        /// The debug step interval in seconds
        /// </summary>
        public float Interval { get; set; } = 1f;

        /// <summary>
        /// Determines whether the script should or not debug position.
        /// </summary>
        public bool DebugPosition { get; set; } = true;

        /// <summary>
        /// Determines whether the script should or not debug rotation.
        /// </summary>
        public bool DebugRotation { get; set; } = true;

        /// <summary>
        /// Determines whether the script should or not debug normal vector.
        /// </summary>
        public bool DebugNormal { get; set; } = false;

        float timeSpent;

        void OnFrame()
        {
            timeSpent += Frame.DeltaTime;

            if (timeSpent >= Interval)
            {
                if (DebugPosition)
                    Debug.Log($"Relative position: {Element.Translation}");

                if (DebugRotation)
                    Debug.Log($"Relative rotation: {Element.Angle}");

                if (DebugNormal)
                    Debug.Log($"Relative normal: {Element.Rotation}");

                Debug.Log();

                timeSpent = 0f;
            }
        }
    }
}
