using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    /// <summary>
    ///     Provides an interface to get time information from GLTech2.
    /// </summary>
    public static class Time // Can be changed.
    {
        // Accessed by Renderer.
        internal static double renderTime = 0f; // Must be 0 after stopping rendering.
        internal static float fixedTime = 0f;

        private static Stopwatch sceneStopwatch = new Stopwatch();
        private static Stopwatch frameStopwatch = new Stopwatch();

        /// <summary>
        ///     Gets the interval in seconds from the last frame to the current one.
        /// </summary>
        public static float DeltaTime => GetTime(frameStopwatch);

        /// <summary>
        ///     Gets how much time, in seconds, have the engine spent rendering since the last Run() was call.
        /// </summary>
        public static float Elapsed => GetTime(sceneStopwatch);

        internal static float FixedTime => fixedTime; // Test

        /// <summary>
        ///     Gets the time in seconds spent only to generate the current frame, not considering time spent processing behaviours.
        /// </summary>
        public static double RenderTime => renderTime;


        internal static void Start()
        {
            sceneStopwatch.Start();
            frameStopwatch.Start();
        }

        internal static void Restart()
        {
            frameStopwatch.Restart();
        }

        /// <summary>
        /// Interrupts measurement of time and redefines the to initial.
        /// </summary>
        internal static void Reset()
        {
            sceneStopwatch.Reset();
            frameStopwatch.Reset();

            renderTime = 0f;
            fixedTime = 0f;
        }


        private static float GetTime(Stopwatch sw) => ((float)sw.ElapsedTicks) / Stopwatch.Frequency;
    }
}
