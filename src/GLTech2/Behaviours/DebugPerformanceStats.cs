using System;

namespace GLTech2.Behaviours
{
    public class DebugPerformanceStats : Behaviour
    {
        float totalFrameTime;
        double totalRenderTime;
        double totalScriptTime;
        float totalWindowTime;
        int frameCount;

        public bool DebugFrameTime { get; set; } = true;

        public bool DebugRenderTime { get; set; } = true;

        public bool DebugScriptTime { get; set; } = true;

        public bool DebugWindowTime { get; set; } = true;

        public float Interval { get; set; } = 4f;

        void OnFrame()
        {
            frameCount++;
            totalFrameTime += Frame.DeltaTime;
            totalRenderTime += Frame.RenderTime;
            totalScriptTime += Frame.ScriptTime;
            totalWindowTime += Frame.WindowTime;

            if (totalFrameTime >= Interval)
            {
                double frameTime = Math.Round(1000.0 * totalFrameTime / frameCount, 2);
                double renderTime = Math.Round(1000.0 * totalRenderTime / frameCount, 2);
                double scriptTime = Math.Round(1000.0 * totalScriptTime / frameCount, 2);
                double windowTime = Math.Round(1000.0 * totalWindowTime / frameCount, 2);

                double frameRate = Math.Round(frameCount / totalFrameTime, 1);
                double renderRate = Math.Round(frameCount / totalRenderTime, 1);
                double scriptRate = Math.Round(frameCount / totalScriptTime, 1);
                double windowRate = Math.Round(frameCount / totalWindowTime, 1);

                if (DebugFrameTime)
                    Debug.Log($"Frame time (avg):\t{frameTime}ms ({frameRate} frames/s)");

                if (DebugRenderTime)
                    Debug.Log($"Render time (avg):\t{renderTime}ms ({renderRate} frames/s)");

                if (DebugScriptTime)
                    Debug.Log($"Script time (avg):\t{scriptTime}ms ({scriptRate} cycles/s) (including this!)");

                if (DebugWindowTime)
                    Debug.Log($"Window time (avg):\t{windowTime}ms ({windowRate} frames/s)");

                Debug.Log();

                totalFrameTime = 0;
                totalRenderTime = 0;
                totalScriptTime = 0;
                totalWindowTime = 0;
                frameCount = 0;
            }
        }
    }
}
