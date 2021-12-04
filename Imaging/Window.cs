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
        Bitmap bitmap;

        public event Action Focus;
        public event Action LoseFocus;
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;

        public unsafe Window(Imaging.Image output, bool fullscreen = false)
        {
            // Setup a bitmap instance that points to the given output buffer
            bitmap = new Bitmap(
                output.Width, output.Height,
                output.Width * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)output.UintBuffer);

            concrete = new ConcreteWindow(bitmap, fullscreen);

            // Setup events
            concrete.KeyDown += (_, args) =>
                KeyDown?.Invoke((InputKey)args.KeyCode);
            concrete.KeyUp += (_, args) =>
                KeyUp?.Invoke((InputKey)args.KeyCode);
            concrete.Click += (_, _) =>   // Bug
                Focus?.Invoke();
            concrete.LostFocus += (_, _) =>
                LoseFocus?.Invoke();
        }

        public bool FullScreen => concrete.FullScreen;
        public (int width, int height) Dimensions => concrete.Dimensions;

        public void Open()
        {
            Focus?.Invoke();
            Application.Run(concrete);
        }

        public void Dispose()
        {
            concrete.Dispose();
            bitmap.Dispose();
        }
    }
}
