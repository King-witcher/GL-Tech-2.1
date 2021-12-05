using System;

namespace Engine.Input
{
    internal interface IKeyboardEventTrigger
    {
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;
    }
}
