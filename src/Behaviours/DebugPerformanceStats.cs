using System;

namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs how much fps the scene is running.
    /// </summary>
    public class DebugPerformanceStats : Behaviour
    {
        float totalFrameTime;
        double totalRenderTime;
        double totalScriptTime;
        int frameCount;

        public bool DebugFrameTime { get; set; } = true;
        public bool DebugRenderTime { get; set; } = true;
        public bool DebugScriptTime { get; set; } = true;

        /// <summary>
        /// The debug step interval in seconds
        /// </summary>
        public float Interval { get; set; } = 1f;

        void Update()
        {
            frameCount++;
            totalFrameTime += Time.DeltaTime;
            totalRenderTime += Time.RenderTime;
            totalScriptTime += Time.ScriptTime;

            if (totalFrameTime >= Interval)
            {
                double frameTime = Math.Round(1000.0 * totalFrameTime / frameCount, 2);
                double renderTime = Math.Round(1000.0 * totalRenderTime / frameCount, 2);
                double scriptTime = Math.Round(1000.0 * totalScriptTime / frameCount, 2);

                double frameRate = Math.Round(frameCount / totalFrameTime, 1);
                double renderRate = Math.Round(frameCount / totalRenderTime, 1);
                double scriptRate = Math.Round(frameCount / totalScriptTime, 1);

                if (DebugFrameTime)
                    Debug.Log($"Frame time (avg):\t{frameTime}ms ({frameRate} frames/s)");

                if (DebugRenderTime)
                    Debug.Log($"Render time (avg):\t{renderTime}ms ({renderRate} frames/s)");

                if (DebugScriptTime)
                Debug.Log($"Script time (avg):\t{scriptTime}ms ({scriptRate} cycles/s)");

                Debug.Log();

                totalFrameTime = 0;
                totalRenderTime = 0;
                totalScriptTime = 0;
                frameCount = 0;
            }
        }
    }
}
