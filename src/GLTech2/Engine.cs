using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GLTech2.Imaging;
using GLTech2.Entities;
using GLTech2.Scripting;
using GLTech2.Unmanaged;

namespace GLTech2
{
    public static partial class Engine
    {
        unsafe static RenderCache* cache;
        static ImageData frontBuffer;
        static Scene activeScene = null;

        // public static bool NativeRendering { get; } = false;

        public static bool ParallelRendering { get; set; } = true;

        public static Scene ActiveScene => activeScene;

        public static event Action OnStart;
        public static event Action OnFrame;

        private static float minframetime = 4;
        public static int MaxFps
        {
            get => (int)(1000f / minframetime);
            set
            {
                Utilities.Clip(ref value, 1, 250);
                minframetime = 1000f / value;
            }
        }

        static bool doubleBuffer = true;
        public static bool DoubleBuffer
        {
            get => doubleBuffer;
            set => ChangeIfNotRunning("DoubleBuffer", ref doubleBuffer, value);
        }

        private static int customWidth = 960;
        public static int CustomWidth
        {
            get => customWidth;
            set => ChangeIfNotRunning("CustomWidth", ref customWidth, value);
        }

        private static int customHeight = 520;
        public static int CustomHeight
        {
            get => customHeight;
            set => ChangeIfNotRunning("CustomHeight", ref customHeight, value);
        }

        static bool fullScreen;
        public static bool FullScreen
        {
            get => fullScreen;
            set
            {
                ChangeIfNotRunning("FullScreen", ref fullScreen, value);
                if (fullScreen == true)
                {
                    CustomWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    customHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                }
            }
        }

        static float fieldOfView = 90f;
        public static float FieldOfView
        {
            get => fieldOfView;
            set
            {
                Utilities.Clip(ref value, 1f, 179f);
                ChangeIfNotRunning("FieldOfView", ref fieldOfView, value);
            }
        }

        static bool captureMouse = false;
        public static bool CaptureMouse
        {
            get => captureMouse;
            set => captureMouse = value;    // Revisar
        }

        public static bool IsRunning { get; private set; } = false;

        public static ImageData GetScreenshot()
        {
            ImageData screenshot = new ImageData(CustomWidth, CustomHeight);
            ImageData.BufferCopy(frontBuffer, screenshot);
            return screenshot;
        }

        public static void AddEffect(ImageProcessing postProcessing)
        {
            Engine.postProcessing.Add(postProcessing);
        }

        public static void AddPostProcessing<T>() where T : ImageProcessing, new()
        {
            AddEffect(new T());
        }

        public unsafe static void Run(Scene scene)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            Camera camera = scene.Camera;

            if (scene == null)
            {
                Debug.InternalLog(
                    message: $"Cannot render a null Scene.",
                    debugOption: Debug.Options.Error);
                return;
            }

            activeScene = scene;

            // Unmanaged buffer where the video will be put.
            frontBuffer = new ImageData(CustomWidth, customHeight);

            // A window that will continuously display the buffer
            MainWindow display = new MainWindow(frontBuffer) { FullScreen = FullScreen };

            // Setup input managers
            display.KeyUp += Behaviour.Keyboard.KeyUp;
            display.KeyDown += Behaviour.Keyboard.KeyDown;
            if (CaptureMouse)
            {
                display.Focus += Behaviour.Mouse.Enable;
                display.LoseFocus += Behaviour.Mouse.Disable;
            }

            // When set to true, the ControlThread will stop rendering.
            var stopRequest = false;

            // And then start the control thread, which is reponsible for distributing the buffer among the threads
            // and running the scene scripts.
            var controlThread = Task.Run(() => ControlTrhead(frontBuffer, in stopRequest));

            // Finally passes control to the rendering screen and displays it.
            display.Start();

            // Theese lines run after the renderer window is closed.
            stopRequest = true;
            controlThread.Wait();

            // Finally, dispose everythihng.
            display.Dispose();
            frontBuffer.Dispose();
            //activeCamera = null;

            IsRunning = false;
        }

        private static unsafe void ReloadCache()
        {
            if (cache != null)
                RenderCache.Delete(cache);
            cache = RenderCache.Create(CustomWidth, CustomHeight, FieldOfView);
        }

        private unsafe static void ControlTrhead(ImageData frontBuffer, in bool cancellationRequest)
        {
            // Spaguetti
            ReloadCache();

            // Buffer where the image will be rendered
            ImageData backBuffer = DoubleBuffer ?
                new ImageData(frontBuffer.Width, frontBuffer.Height) :
                frontBuffer;

            #region Warnings
            if (!DoubleBuffer && postProcessing.Count > 0)
                Debug.InternalLog(
                    message: "The renderer has post processing effects set but DoubleBuffering is disabled. " +
                        "Post processing effects may not work properly.",
                    debugOption: Debug.Options.Warning);
            #endregion

            Stopwatch controlStopwatch = new Stopwatch();   // Required to cap framerate
            Behaviour.Frame.RestartFrame();
            Behaviour.Frame.BeginScript();
            activeScene.Start?.Invoke();
            Behaviour.Frame.EndScript();

            while (!cancellationRequest)
            {
                controlStopwatch.Restart();
                Behaviour.Frame.BeginRender();

                DrawPlanes(backBuffer, activeScene.unmanaged);
                PostProcess(backBuffer);

                if (DoubleBuffer)
                    ImageData.BufferCopy(backBuffer, frontBuffer);
                Behaviour.Frame.EndRender();

                while (controlStopwatch.ElapsedMilliseconds < minframetime)
                    Thread.Yield();

                Behaviour.Frame.RestartFrame();
                Behaviour.Frame.BeginScript();
                activeScene.OnFrame?.Invoke();
                Behaviour.Frame.EndScript();
            }
            controlStopwatch.Stop();
            Behaviour.Frame.Stop();

            if (DoubleBuffer)
                backBuffer.Dispose();
            return;
        }

        private static List<ImageProcessing> postProcessing = new List<ImageProcessing>();
        private static void PostProcess(ImageData target)
        {
            foreach (var effect in postProcessing)
                effect.Process(target);
        }

        static void ChangeIfNotRunning<T>(string name, ref T obj, T value)
        {
            if (IsRunning)
                Debug.InternalLog(
                    message: $"The value of \"{name}\" cannot be modified while running. Value will keep \"{obj}\".",
                    debugOption: Debug.Options.Warning);
            else
                obj = value;
        }
    }
}