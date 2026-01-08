namespace GLTech.Scripting
{
    partial class Script
    {
        protected internal static class Time
        {
            private static double scriptTime = 0.0;

            public static long FixedTicks { get; set; } = 50;

            public static float FixedTimestep => 1.0f / FixedTicks;

            public static float TotalTime { get; internal set; }

            public static float TimeStep { get; internal set; }

            public static float FixedRemainder { get; internal set; }

            public static double RenderTime { get; internal set; }

            public static double ScriptTime => scriptTime;

            public static float WindowTime { get; internal set; }

        }
    }
}