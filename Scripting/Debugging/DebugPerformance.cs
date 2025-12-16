using System;

namespace Engine.Scripting.Debugging
{
    public class DebugPerformance : Script
    {
        static Logger logger = new Logger(typeof(DebugPerformance).Name);
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

        public void OnFrame()
        {
            frameCount++;
            totalFrameTime += Time.TimeStep;
            totalRenderTime += Time.RenderTime;
            totalScriptTime += Time.ScriptTime;
            totalWindowTime += Time.WindowTime;

            if (totalFrameTime < Interval)
                return;

            float frameTime = MathF.Round(1000f * totalFrameTime / frameCount, 2);
            double renderTime = Math.Round(1000.0 * totalRenderTime / frameCount, 2);
            double scriptTime = Math.Round(1000.0 * totalScriptTime / frameCount, 2);
            double windowTime = Math.Round(1000.0 * totalWindowTime / frameCount, 2);

            float frameRate = MathF.Round(frameCount / totalFrameTime, 1);
            double renderRate = Math.Round(frameCount / totalRenderTime, 1);
            double scriptRate = Math.Round(frameCount / totalScriptTime, 1);
            double windowRate = Math.Round(frameCount / totalWindowTime, 1);

            if (DebugFrameTime)
                logger.Log($"Frame time (avg): {frameTime}ms ({frameRate} frames/s)");

            if (DebugRenderTime)
                logger.Log($"Render time (avg): {renderTime}ms ({renderRate} frames/s)");

            if (DebugScriptTime)
                logger.Log($"Script time (avg): {scriptTime}ms ({scriptRate} cycles/s) (including this!)");

            if (DebugWindowTime)
                logger.Log($"Window time (avg): {windowTime}ms ({windowRate} frames/s)");

            totalFrameTime = 0;
            totalRenderTime = 0;
            totalScriptTime = 0;
            totalWindowTime = 0;
            frameCount = 0;
        }
    }
}
