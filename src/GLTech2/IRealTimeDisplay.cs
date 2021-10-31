using System;

using GLTech2.Scripting;

namespace GLTech2
{
    internal interface IInputReceiver
    {
        event Action Focus;
        event Action LoseFocus;
        event Action<InputKey> KeyDown;
        event Action<InputKey> KeyUp;
    }

    internal interface IRealTimeDisplay
    {
        bool FullScreen { get; set; }
        (int width, int height) Dimensions { get; set; }
        void Start();
    }
}
