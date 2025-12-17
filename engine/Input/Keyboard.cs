using System;
using System.Collections.Generic;

namespace Engine.Input
{
    public static class Keyboard
    {
        static HashSet<ScanCode> keysDown = new();

        public static bool IsKeyDown(ScanCode key) => keysDown.Contains(key);

        // Chamado por Renderer
        internal static void SetKeyDown(ScanCode key)
        {
            keysDown.Add(key);
        }

        // Chamado por Renderer
        internal static void SetKeyUp(ScanCode key)
        {
            keysDown.Remove(key);
        }

        internal static void Clear() => keysDown.Clear();
    }
}