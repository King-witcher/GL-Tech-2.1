using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using GLTech2.Drawing;

namespace GLTech2
{
    public static partial class Renderer
    {
        unsafe static RenderCache* cache;
        static PixelBuffer frontBuffer;
        static Scene activeScene = null;
        static Camera activeCamera = null;

        // public static bool NativeRendering { get; } = false;

        public static bool ParallelRendering { get; set; } = true;

        public static Scene ActiveScene => activeScene;

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
                    CustomWidth = Screen.PrimaryScreen.Bounds.Width;
                    customHeight = Screen.PrimaryScreen.Bounds.Height;
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

        public static PixelBuffer GetScreenshot()
        {
            PixelBuffer pb = new PixelBuffer(CustomWidth, customHeight);
            pb.Clone(frontBuffer);
            return pb;
        }

        public static void AddEffect(PostProcessing postProcessing)
        {
            Renderer.postProcessing.Add(postProcessing);
        }

        public static void AddPostProcessing<T>() where T : PostProcessing, new()
        {
            AddEffect(new T());
        }

        public unsafe static void Start(Scene scene)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            Camera camera = scene.Camera;

            if (scene == null)
            {
                Debug.InternalLog(
                    message: $"Cannot render from Camera \"{camera}\" because it's not bound to any Scene.",
                    debugOption: Debug.Options.Error);
                return;
            }

            activeCamera = camera;

            activeScene = scene;

            // Unmanaged buffer where the video will be put.
            frontBuffer = new PixelBuffer(CustomWidth, customHeight);

            // Create a wrapper "Bitmap" that uses the same buffer.
            var sourceBitmap = new Bitmap(
                CustomWidth, CustomHeight,
                CustomWidth * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)frontBuffer.uint0);

            var display = new Display(FullScreen, CustomWidth, CustomHeight, sourceBitmap);

            // We must define two booleans to communicate with the tread.
            // The first is necessary to send a stop request.
            // The second is necessary to be aware of when the renderer doesn't need our unmanaged resources and
            // then be able to realease them all.
            var stopRequest = false;
            var controlThreadRunning = true;

            // And then start the control thread, which is reponsible for distributing the buffer among the threads
            // and running the scene scripts.
            var controlThread = Task.Run(() => ControlTrhead(frontBuffer, in stopRequest, ref controlThreadRunning));

            // Finally passes control to the rendering screen and displays it.
            Application.Run(display);

            // Theese lines run after the renderer window is closed.
            stopRequest = true;

            // Wait for the control thread to stop using outputBuffer.
            while (controlThreadRunning)
                Thread.Yield();

            // Finally, dispose everythihng.
            display.Dispose();
            frontBuffer.Dispose();
            sourceBitmap.Dispose();
            activeCamera = null;

            IsRunning = false;
        }

        private static unsafe void ReloadCache()
        {
            if (cache != null)
                RenderCache.Delete(cache);
            cache = RenderCache.Create(CustomWidth, CustomHeight, FieldOfView);
        }

        private unsafe static void ControlTrhead(PixelBuffer frontBuffer, in bool cancellationRequest, ref bool controlThreadRunning)
        {
            ReloadCache();

            // Buffer where the image will be rendered
            PixelBuffer backBuffer = DoubleBuffer ?
                new PixelBuffer(frontBuffer.width, frontBuffer.height) :
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

            // While this variable is set to true, outputBuffer cannot be released by Renderer.Run() thread.
            controlThreadRunning = true;

            while (!cancellationRequest)
            {
                controlStopwatch.Restart();
                Behaviour.Frame.BeginRender();

                DrawPlanes(backBuffer, activeScene.unmanaged);
                PostProcess(backBuffer);

                if (DoubleBuffer)
                    frontBuffer.FastClone(backBuffer);
                Behaviour.Frame.EndRender();

                while (controlStopwatch.ElapsedMilliseconds < minframetime)
                    Thread.Yield();

                Behaviour.Mouse.Measure();
                Behaviour.Frame.RestartFrame();
                Behaviour.Frame.BeginScript();
                activeScene.OnFrame?.Invoke();
                Behaviour.Frame.EndScript();
            }
            controlStopwatch.Stop();
            Behaviour.Frame.Stop();

            // FrontBuffer is up to be released, if used.
            controlThreadRunning = false;
            if (DoubleBuffer)
                backBuffer.Dispose();
            return;
        }

        private static List<PostProcessing> postProcessing = new List<PostProcessing>();
        private static void PostProcess(PixelBuffer target)
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