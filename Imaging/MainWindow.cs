using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine.Imaging;
using Engine.Scripting;

namespace Engine
{
    public partial class MainWindow : IDisposable
    {
        GLTechWindowForm form;
        Bitmap bitmap;

        public event Action Focus;
        public event Action LoseFocus;
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;

        public bool FullScreen
        {
            get => form.Maximize;
            set => form.Maximize = value;
        }

        public (int width, int height) Dimensions
        {
            get => form.Dimensions;
            set => form.Dimensions = value;
        }

        public unsafe MainWindow(Imaging.Image output)
        {
            // Setup a bitmap instance that points to the given output buffer
            bitmap = new Bitmap(
                output.Width, output.Height,
                output.Width * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)output.UintBuffer);

            form = new GLTechWindowForm(bitmap);

            // Setup events
            form.KeyDown += (_, args) =>
                KeyDown?.Invoke((InputKey)args.KeyCode);
            form.KeyUp += (_, args) =>
                KeyUp?.Invoke((InputKey)args.KeyCode);
            form.Click += (_, _) =>   // Bug
                Focus?.Invoke();
            form.LostFocus += (_, _) =>
                LoseFocus?.Invoke();
        }

        public void Start()
        {
            Focus?.Invoke();
            Application.Run(form);
        }

        public void Dispose()
        {
            form.Dispose();
            bitmap.Dispose();
        }
    }
}
