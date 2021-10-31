using System;

using GLTech2.Scripting;

namespace GLTech2
{
    internal interface IInputProvider
    {
        event Action Focus;
        event Action LoseFocus;
        event Action<InputKey> KeyDown;
        event Action<InputKey> KeyUp;
    }
}
