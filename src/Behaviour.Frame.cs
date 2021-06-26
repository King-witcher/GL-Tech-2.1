using System.Diagnostics;

namespace GLTech2
{
    partial class Behaviour
    {
        /// <summary>
        /// Provides an interface to get time information about the last frame.
        /// </summary>
        protected internal static class Frame
        {
            private static Stopwatch frameStopwatch = new Stopwatch();
            private static Stopwatch renderStopwatch = new Stopwatch();
            private static Stopwatch scriptStopwatch = new Stopwatch();
            private static Stopwatch windowStopwatch = new Stopwatch();

            private static float frameTime = 0f;
            private static double renderTime = 0.0;
            private static double scriptTime = 0.0;
            private static float windowTime = 0f;

            /// <summary>
            /// The same as FrameTime. Similar to Unity3D.
            /// </summary>
            public static float DeltaTime => Time;

            /// <summary>
            /// Gets the interval in seconds from the last frame to the current one.
            /// </summary>
            public static float Time => frameTime;

            /// <summary>
            /// Gets the time in seconds spent only to generate the current frame, not considering time spent running behaviours.
            /// </summary>
            public static double RenderTime => renderTime;

            /// <summary>
            /// Gets the time in seconds spent only to run all behaviour scripts in the previous frame, not considering time spent rendering.
            /// </summary>
            public static double ScriptTime => scriptTime;

            /// <summary>
            /// Gets the time in seconds spent to update the last window frame.
            /// </summary>
            public static float WindowTime => windowTime;

            #region This region interface is used by Renderer
            internal static void RestartFrame()
            {
                frameTime = (float)frameStopwatch.ElapsedTicks / Stopwatch.Frequency;
                frameStopwatch.Restart();
            }

            internal static void BeginRender()
            {
                renderStopwatch.Restart();
            }

            internal static void EndRender()
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

            internal static void BeginWindow()
            {
                windowStopwatch.Restart();
            }

            internal static void EndWindow()
            {
                windowStopwatch.Stop();
                windowTime = (float)windowStopwatch.ElapsedTicks / Stopwatch.Frequency;
            }

            internal static void Stop()
            {
                scriptStopwatch.Reset();
                renderStopwatch.Reset();
                frameStopwatch.Reset();
                windowStopwatch.Reset();

                frameTime = 0;
                renderTime = 0;
                scriptTime = 0;
                windowTime = 0;
            }
            #endregion

            private static float GetTime(Stopwatch sw) => (float)sw.ElapsedTicks / Stopwatch.Frequency;
        }
    }
}