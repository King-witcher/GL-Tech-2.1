using System;
using System.Collections.Generic;

namespace Engine.Input
{
    public static class Keyboard
    {
        static HashSet<InputKey> keysDown = new();

        public static bool IsKeyDown(InputKey key) => keysDown.Contains(key);

        // Chamado por Renderer
        internal static void SetKeyDown(InputKey key)
        {
            keysDown.Add(key);
        }

        // Chamado por Renderer
        internal static void SetKeyUp(InputKey key)
        {
            keysDown.Remove(key);
        }

        internal static void Clear() => keysDown.Clear();
    }
}