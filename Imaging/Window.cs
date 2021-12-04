using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine.Imaging;
using Engine.Scripting;

namespace Engine
{
    public partial class Window : IDisposable
    {
        ConcreteWindow concrete;

        public unsafe Window(Imaging.Image output, bool fullscreen = false)
        {
            concrete = new ConcreteWindow((Bitmap)output, fullscreen);

            // Setup events
            concrete.KeyDown += (_, args) => KeyDownAction?.Invoke((InputKey)args.KeyCode);
            concrete.KeyUp += (_, args) => KeyUpAction?.Invoke((InputKey)args.KeyCode);
            concrete.GotFocus += (_, _) => Focus?.Invoke();
            concrete.LostFocus += (_, _) => Unfocus?.Invoke();
        }

        public event Action Focus;
        public event Action Unfocus;

        // Isso funciona dessa forma para conseguir personalizar a interface do evento mas ainda assim ser capaz de remover depois.
        public event Action<InputKey> KeyDown
        {
            add => KeyDownAction += value;
            remove => KeyDownAction -= value;
        }
        private Action<InputKey> KeyDownAction;

        public event Action<InputKey> KeyUp
        {
            add => KeyUpAction += value;
            remove => KeyUpAction -= value;
        }
        private Action<InputKey> KeyUpAction;

        public bool FullScreen => concrete.FullScreen;
        public (int width, int height) Dimensions => concrete.Dimensions;

        public void Open()
        {
            Focus?.Invoke();
            Application.Run(concrete);
        }

        // TODO checar se isso não causa vazamento de memória
        public void Dispose()
        {
            concrete.Dispose();
            // bitmap.Dispose();
        }
    }
}
