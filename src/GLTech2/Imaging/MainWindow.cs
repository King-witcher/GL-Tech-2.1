﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using GLTech2.Imaging;
using GLTech2.Scripting;

namespace GLTech2
{
    public partial class MainWindow : IRealTimeDisplay, IInputReceiver, IDisposable
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

        public unsafe MainWindow(PixelBuffer output)
        {
            // Setup a bitmap instance that points to the given output buffer
            bitmap = new Bitmap(
                output.width, output.height,
                output.width * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)output.Uint0);

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