using System;
using System.Collections.Generic;

namespace GLTech2.Scripting
{
    public partial class Behaviour
    {
        protected internal static class Keyboard
        {
            static LinkedList<InputKey> Keys = new LinkedList<InputKey>();

            public static bool IsKeyDown(InputKey key) => Keys.Contains(key);
            public static event Action<InputKey> OnKeyDown;
            public static event Action<InputKey> OnKeyUp;

            internal static void KeyDown(InputKey key)
            {
                Keys.AddFirst(key);
                OnKeyDown?.Invoke(key);
            }

            internal static void KeyUp(InputKey key)
            {
                while (Keys.Contains(key))  // Suboptimal
                    Keys.Remove(key);
                OnKeyUp?.Invoke(key);
            }
        }
    }
}
