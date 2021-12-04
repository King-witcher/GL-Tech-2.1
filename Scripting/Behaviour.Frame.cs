using System.Diagnostics;

namespace Engine.Scripting
{
    partial class Behaviour
    {
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

            public static float DeltaTime => frameTime;

            public static double RenderTime => renderTime;

            public static double ScriptTime => scriptTime;

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