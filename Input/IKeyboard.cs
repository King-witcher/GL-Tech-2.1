using System;

namespace Engine.Input
{
    internal interface IKeyboard
    {
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;
    }
}
