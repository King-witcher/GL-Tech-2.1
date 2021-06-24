namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs all relative components of an object.
    /// </summary>
    public class DebugPosition : Behaviour
    {
        float interval;
        /// <summary>
        /// The debug step interval in seconds
        /// </summary>
        public float Interval { get; set; } = 1f;

        void Update()
        {
            interval += Time.DeltaTime;

            if (interval >= Interval)
            {
                Debug.Log($"Relative position: {Element.Position}");
                Debug.Log($"Relative rotation: {Element.Rotation}");
                Debug.Log($"Relative normal: {Element.Normal}");
                Debug.Log();
                interval = 0f;
            }
        }
    }
}
