using System.Collections.Generic;

namespace Engine.Input
{
    public static class Keyboard
    {
        static HashSet<InputKey> pressedKeys = new();
        static IKeyboardEventTrigger trigger;

        public static bool IsKeyDown(InputKey key) => pressedKeys.Contains(key);

        internal static void Assign(IKeyboardEventTrigger trigger)
        {
            static void KeyDown(InputKey key) => pressedKeys.Add(key);
            static void KeyUp(InputKey key) => pressedKeys.Remove(key);

            if (Keyboard.trigger != null)
            {
                trigger.KeyDown -= KeyDown;
                trigger.KeyUp -= KeyUp;
            }

            Keyboard.trigger = trigger;
            trigger.KeyDown += KeyDown;
            trigger.KeyUp += KeyUp;
        }
    }
}