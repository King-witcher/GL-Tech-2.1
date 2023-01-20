using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Engine.Input;

namespace Engine
{
    public partial class Canvas : IDisposable, IKeyboard
    {
        CanvasWindow concrete;
        HashSet<InputKey> pressedKeys = new();

        public event Action Focus;
        public event Action Unfocus;
        public event Action<InputKey> KeyDown;
        public event Action<InputKey> KeyUp;

        public unsafe Canvas(Imaging.Image output, bool fullscreen = false)
        {
            concrete = new CanvasWindow((Bitmap)output, fullscreen);

            concrete.KeyDown += TreatKeyDown;
            concrete.KeyUp += TreatKeyUp;
            concrete.GotFocus += (_, _) => Focus?.Invoke();
            concrete.LostFocus += (_, _) => Unfocus?.Invoke();
        }

        public bool Maximized => concrete.Maximized;
        public (int width, int height) Dimensions => concrete.Dimensions;

        private void TreatKeyDown(object sender, KeyEventArgs args)
        {
            InputKey key = (InputKey)args.KeyCode;
            if (!pressedKeys.Contains(key))
            {
                pressedKeys.Add(key);
                KeyDown?.Invoke(key);
            }
        }

        private void TreatKeyUp(object sender, KeyEventArgs args)
        {
            InputKey key = (InputKey)args.KeyCode;
            if (pressedKeys.Contains(key))
            {
                pressedKeys.Remove(key);
                KeyUp?.Invoke(key);
            }
        }

        public void Open()
        {
            Focus?.Invoke();
            Application.Run(concrete);
        }

        public void Dispose()
        {
            Focus = null;
            Unfocus = null;
            KeyUp = null;
            KeyDown = null;
            concrete.Dispose();

            // TODO checar se isso não causa vazamento de memória com a imagem
            // bitmap.Dispose();
        }
    }
}
