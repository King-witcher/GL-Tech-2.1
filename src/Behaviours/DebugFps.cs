namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs how much fps the scene is running.
    /// </summary>
    public class DebugFPS : Behaviour
    {
        double totalTime;
        float frameTime;
        int frameCount;

        /// <summary>
        /// The debug step interval in seconds
        /// </summary>
        public float Interval { get; set; } = 1f;

        void Update()
        {
            frameCount++;
            totalTime += Time.RenderTime;
            frameTime += Time.DeltaTime;

            if (frameTime >= Interval)
            {
                Debug.Log("Raycast only fps: \t" + frameCount / totalTime);
                Debug.Log("Full time fps: \t" + frameCount / frameTime);
                Debug.Log();
                frameCount = 0;
                totalTime = 0.0;
                frameTime = 0f;
            }
        }
    }
}
