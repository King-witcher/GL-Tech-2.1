using System;
using System.Drawing;
using System.Windows.Forms;
using Engine.Scripting;
using Engine.Input;

namespace Engine
{
    public partial class Window : IDisposable, IKeyboardEventTrigger
    {
        ConcreteWindow concrete;

        public unsafe Window(Imaging.Image output, bool fullscreen = false)
        {
            concrete = new ConcreteWindow((Bitmap)output, fullscreen);

            concrete.KeyDown += (_, args) => KeyDown?.Invoke((InputKey)args.KeyCode);
            concrete.KeyUp += (_, args) => KeyUp?.Invoke((InputKey)args.KeyCode);
            concrete.GotFocus += (_, _) => Focus?.Invoke();
            concrete.LostFocus += (_, _) => Unfocus?.Invoke();
        }

        public event Action Focus;
        public event Action Unfocus;
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;

        public bool Maximized => concrete.Maximized;
        public (int width, int height) Dimensions => concrete.Dimensions;

        public void Open()
        {
            Focus?.Invoke();
            Application.Run(concrete);
        }

        // TODO checar se isso não causa vazamento de memória com a imagem
        public void Dispose()
        {
            concrete.Dispose();
            // bitmap.Dispose();
        }
    }
}
