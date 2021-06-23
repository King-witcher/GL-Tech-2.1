using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal static unsafe class Utilities
    {

        internal const float ToRad = (float) Math.PI / 180f;
        internal const float ToDegree = 180f / (float)Math.PI;

        internal static void Clip<T>(ref T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            else if (value.CompareTo(min) < 0)
                value = min;
        }
    }
}
