namespace Engine.Scripting
{
    partial class Script
    {
        protected internal static class Time
        {
            public const long FIXED_TICKS_PER_SECOND = 50;

            private static double scriptTime = 0.0;

            public static float TimeStep { get; internal set; }

            public static float FixedRemainder { get; internal set; }

            public static double RenderTime { get; internal set; }

            public static double ScriptTime => scriptTime;

            public static float WindowTime { get; internal set; }

        }
    }
}