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

            concrete.KeyDown += (_, args) => KeyDown?.Invoke((InputKey)args.KeyCode);
            concrete.KeyUp += (_, args) => KeyUp?.Invoke((InputKey)args.KeyCode);
            concrete.GotFocus += (_, _) => Focus?.Invoke();
            concrete.LostFocus += (_, _) => Unfocus?.Invoke();
        }

        public event Action Focus;
        public event Action Unfocus;
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;

        public bool FullScreen => concrete.FullScreen;
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
