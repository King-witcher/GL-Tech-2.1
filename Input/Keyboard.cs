using System;
using System.Collections.Generic;

namespace Engine.Input
{
    public static class Keyboard
    {
        // Significa que algum teclado está sendo lido
        public static bool Assigned { get; private set; } = false;
        static HashSet<InputKey> pressedKeys = new();
        static IKeyboard keyboard;

        // TODO fazer uma verificação mais cautelosa aqui.
        public static bool IsKeyDown(InputKey key) => pressedKeys.Contains(key);

        private static void SetKeyDown(InputKey key)
        {
            if (!IsKeyDown(key))
            {
                pressedKeys.Add(key);
                OnKeyDown?.Invoke(key);
            }
        }

        private static void SetKeyUp(InputKey key)
        {
            if (IsKeyDown(key))
            {
                pressedKeys.Remove(key);
                OnKeyUp?.Invoke(key);
            }
        }

        internal static void Assign(IKeyboard keyboard)
        {
            if (Assigned)
                Unassign();

            Keyboard.keyboard = keyboard;
            keyboard.KeyDown += SetKeyDown;
            keyboard.KeyUp += SetKeyUp;

            Assigned = true;
        }

        internal static event Action<InputKey> OnKeyDown;
        internal static event Action<InputKey> OnKeyUp;

        internal static void Unassign()
        {
            keyboard.KeyDown -= SetKeyDown;
            keyboard.KeyUp -= SetKeyUp;
            keyboard = null;
            pressedKeys.Clear();

            Assigned = false;
        }
    }
}