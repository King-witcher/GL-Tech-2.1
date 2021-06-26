using System.Diagnostics;

namespace GLTech2
{
    /// <summary>
    /// Provides an interface to get time information from GLTech2.
    /// </summary>
    public static class Time
    {
        private static Stopwatch frameStopwatch = new Stopwatch();
        private static Stopwatch renderStopwatch = new Stopwatch();
        private static Stopwatch sceneStopwatch = new Stopwatch();
        private static Stopwatch scriptStopwatch = new Stopwatch();

        private static double renderTime = 0.0;
        private static double scriptTime = 0.0;

        /// <summary>
        /// The same as FrameTime. Similar to Unity3D.
        /// </summary>
        public static float DeltaTime => FrameTime;

        /// <summary>
        /// Gets how much time, in seconds, have the engine spent rendering since the last Run() was call.
        /// </summary>
        public static float Elapsed => GetTime(sceneStopwatch);

        /// <summary>
        /// Gets the interval in seconds from the last frame to the current one.
        /// </summary>
        public static float FrameTime => GetTime(frameStopwatch);

        /// <summary>
        /// Gets the time in seconds spent only to generate the current frame, not considering time spent processing behaviours.
        /// </summary>
        public static double RenderTime => renderTime;

        /// <summary>
        /// Gets the time in seconds spent only to run all behaviour scripts in the previous frame, not considering time spent rendering.
        /// </summary>
        public static double ScriptTime => scriptTime;

        #region This region interface is used by Renderer
        internal static void BeginRunning()
        {
            sceneStopwatch.Start();
            frameStopwatch.Start();
        }

        internal static void StopRunning()
        {
            sceneStopwatch.Reset();
            frameStopwatch.Reset();

            renderTime = 0;
            scriptTime = 0;
        }

        internal static void RestartFrame()
        {
            frameStopwatch.Restart();
        }

        internal static void BeginRender()
        {
            renderStopwatch.Restart();
        }

        internal static void StopRender()
        {
            renderStopwatch.Stop();
            renderTime = (double)renderStopwatch.ElapsedTicks / Stopwatch.Frequency;
        }

        internal static void BeginScript()
        {
            scriptStopwatch.Restart();
        }

        internal static void EndScript()
        {
            scriptStopwatch.Stop();
            scriptTime = (double)scriptStopwatch.ElapsedTicks / Stopwatch.Frequency;
        }
        #endregion

        private static float GetTime(Stopwatch sw) => ((float)sw.ElapsedTicks) / Stopwatch.Frequency;
    }
}
