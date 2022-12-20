using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Engine.Imaging;
using Engine.Imaging.Processing;
using Engine.World;
using Engine.Scripting;
using Engine.Data;
using System.Runtime.CompilerServices;

namespace Engine
{
    public static partial class Renderer
    {
        unsafe static RenderCache* cache;
        static Image frontBuffer;
        static Scene currentScene = null;
        static Action ScheduledActions = null;
        public static bool ParallelRendering { get; set; } = true;

        public static Scene ActiveScene => currentScene;

        private static float minframetime = 4;
        public static int MaxFps
        {
            get => (int)(1000f / minframetime);
            set
            {
                Util.Clip(ref value, 1, 250);
                minframetime = 1000f / value;
            }
        }

        static bool doubleBuffer = true;
        public static bool SynchronizeThreads
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
                Util.Clip(ref value, 1f, 179f);
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

        public static Image GetScreenshot()
        {
            Image screenshot = new Image(CustomWidth, CustomHeight);
            Image.BufferCopy(frontBuffer, screenshot);
            return screenshot;
        }

        public static void AddEffect(Effect effect)
        {
            Renderer.postProcessing.Add(effect);
        }

        public static void AddEffect<EffectClass>() where EffectClass : Effect, new()
        {
            AddEffect(new EffectClass());
        }

        public unsafe static void Run(Scene scene)
        {
            #region Checks
            if (IsRunning)
                return;
            IsRunning = true;

            if (scene == null)
            {
                Debug.InternalLog(
                    message: $"Cannot render a null Scene.",
                    debugOption: Debug.Options.Error);
                return;
            }

            if (scene.Background.source.Buffer == IntPtr.Zero)
                Debug.InternalLog(
                    message: $"The Scene being rendered does not have a background texture. Add it by using Scene.Background property.",
                    debugOption: Debug.Options.Warning);
            #endregion

            Camera camera = scene.Camera;
            currentScene = scene;

            // Unmanaged buffer where the video will be put.
            frontBuffer = new(CustomWidth, CustomHeight);

            // A window that will continuously display the buffer
            WindowAdapter window = new(frontBuffer, FullScreen);

            // Setup input managers
            if (CaptureMouse)
            {
                window.Focus += Input.Mouse.Enable;
                window.Unfocus += Input.Mouse.Disable;
            }

            // These parts are scheduled to prevent user scripts from running while the engine  is rendering the scene.
            window.KeyDown += key => Schedule(() =>
            {
                Input.Keyboard.SetKeyDown(key);
                scene.OnKeyDown?.Invoke(key);
            });

            window.KeyUp += key => Schedule(() =>
            {
                Input.Keyboard.SetKeyUp(key);
                scene.OnKeyUp?.Invoke(key);
            });

            // When set to true, the ControlThread will render for the last time and stop.
            var stopRequested = false;

            // And then start the control thread, which is reponsible for distributing the buffer among the threads
            // and running the scene scripts.
            var controlThread = Task.Run(() => ControlTrhead(frontBuffer, in stopRequested));

            // Finally, passes control to the rendering screen and displays it.
            window.Open();

            // Theese lines run after the renderer window is closed.
            stopRequested = true;
            controlThread.Wait();

            // Clears the Keyboard
            Input.Keyboard.Clear();

            // Finally, dispose everythihng.
            window.Dispose();
            frontBuffer.Dispose();

            currentScene = null;
            IsRunning = false;
        }

        private static unsafe void RefreshCache()
        {
            if (cache != null)
                RenderCache.Delete(cache);
            cache = RenderCache.Create(CustomWidth, CustomHeight, FieldOfView);
        }

        private static void Schedule(Action action) => ScheduledActions += action;

        // Executa ações que foram agendadas enquanto a engine renderizava.
        private static void RunScheduled()
        {
            ScheduledActions?.Invoke();
            ScheduledActions = null;
        }

        private unsafe static void ControlTrhead(Image frontBuffer, in bool stopRequested)
        {
            // Spaguetti
            RefreshCache();

            // Buffer where the image will be rendered
            Image backBuffer = SynchronizeThreads ?
                new (frontBuffer.Width, frontBuffer.Height) :
                frontBuffer;

            #region Warnings
            if (!SynchronizeThreads && postProcessing.Count > 0)
                Debug.InternalLog(
                    message: "The renderer has post processing effects set but DoubleBuffering is disabled. " +
                        "Post processing effects may not work properly.",
                    debugOption: Debug.Options.Warning);
            #endregion

            Stopwatch controlStopwatch = new Stopwatch();   // Required to cap framerate
            Script.Frame.RestartFrame();
            Script.Frame.BeginScript();
            currentScene.Start?.Invoke();
            Script.Frame.EndScript();

            while (!stopRequested)
            {
                controlStopwatch.Restart();
                Script.Frame.BeginRender();

                Draw(backBuffer, currentScene.unmanaged);
                //DrawFloors(backBuffer, currentScene.unmanaged);
                //PostProcess(backBuffer);

                if (SynchronizeThreads)
                    Image.BufferCopy(backBuffer, frontBuffer);
                Script.Frame.EndRender();

                while (controlStopwatch.ElapsedMilliseconds < minframetime)
                    Thread.Yield();

                RunScheduled();
                Script.Frame.RestartFrame();
                Script.Frame.BeginScript();
                currentScene.OnFrame?.Invoke();
                Script.Frame.EndScript();
            }
            controlStopwatch.Stop();
            Script.Frame.Stop();

            if (SynchronizeThreads)
                backBuffer.Dispose();
            return;
        }

        // EXTREMELLY suboptimal; spike version
        private unsafe static void DrawFloors(Image buffer, SScene* scene)
        {
            // Checks if the code should be ran in all cores or just one.
            if (ParallelRendering)
                Parallel.For(fromInclusive: 0, toExclusive: buffer.Height, body: DrawLine);
            else
                for (int i = 0; i < buffer.Height; i++)
                    DrawLine(i);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void DrawLine(int screen_line)
            {
                if (screen_line > buffer.Height >> 1)
                {
                    float pre_dist = buffer.flt_width / (2 * buffer.flt_height * (float)Math.Tan(Util.ToRad * fieldOfView / 2f));
                    float post_dist = pre_dist * (buffer.flt_height - screen_line - 0.5f) / (screen_line - buffer.flt_height / 2 + 0.5f);
                    float fall_dist = pre_dist + post_dist;

                    Vector camera_dir = new(scene->camera->rotation);
                    Vector center_floor_hit = scene->camera->position + camera_dir * fall_dist;

                    float scratio = buffer.flt_width / buffer.flt_height;
                    float factor = cache->fall_factors[screen_line];

                    Vector lr_direction = new Vector(camera_dir.Y, -camera_dir.X) * scratio * factor;

                    Vector left_floor_hit = center_floor_hit - lr_direction * 0.5f;

                    float step = 1 / buffer.flt_width;

                    SFloor* strf = null;

                    for (int screen_column = 0; screen_column < buffer.Width; screen_column++)
                    {
                        Vector point = left_floor_hit + screen_column * step * lr_direction;
                        SFloor* current = scene->FloorAt(point);
                        if (strf != null && current != strf)
                        if (strf != null)
                        {
                            buffer[screen_column, screen_line] = strf->MapTexture(point);
                        }
                    }
                }
            }
        }

        private unsafe static void Draw(Image screen, SScene* scene)
        {
            // Checks if the code should be run in all cores or just one.
            if (ParallelRendering)
                Parallel.For(fromInclusive: 0, toExclusive: screen.Width, body: DrawColumn);
            else
                for (int i = 0; i < screen.Width; i++)
                    DrawColumn(i);

            // Checks if the code should be run in all cores or just one.
            if (ParallelRendering)
                Parallel.For(fromInclusive: screen.Height >> 1, toExclusive: screen.Height, body: drawFloorLine);
            else
                for (int i = screen.Height >> 1; i < screen.Height; i++)
                    drawFloorLine(i);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void drawFloorLine(int line)
            {
                float fall_dist = cache->fall_dists[line];

                Vector camera_dir = new(scene->camera->rotation);

                Vector center_floor_hit = scene->camera->position + camera_dir * fall_dist;
                float scratio = screen.flt_width / screen.flt_height;
                float factor = cache->fall_factors[line];
                Vector lr_direction = new Vector(camera_dir.Y, -camera_dir.X) * scratio * factor;
                Vector left_floor_hit = center_floor_hit - lr_direction * 0.5f;

                SFloorList list = scene->floor_list.GetIntersections(left_floor_hit, left_floor_hit + lr_direction);

                for (int i = 0; i < screen.Width; i++)
                {
                    Vector point = left_floor_hit + i * lr_direction / screen.flt_width;

                    SFloor* strf = list.Locate(point);
                    if (strf != null)
                    {
                        screen[i, line] = strf->MapTexture(point);
                    }
                    else
                    {
                        screen[i, line] = 0;
                    }
                }

                list.Dispose();
            }

            // Render a vertical column of the screen.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void DrawColumn(int screen_column)
            {
                unchecked
                {
                    // Caching frequently used variables
                    float ray_cos = cache->cosines[screen_column];
                    float ray_angle = cache->angles[screen_column] + scene->camera->rotation;
                    Texture background = scene->background;
                    Ray ray = new Ray(scene->camera->position, ray_angle);

                    // Cast the ray towards every plane.
                    PlaneStruct* nearest = scene->NearestPlane(ray, out float nearest_dist, out float nearest_ratio);

                    // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                    #region Render the plane

                    // Height that the current column should have on the screen.
                    float columnHeight = (cache->colHeight1 / (ray_cos * nearest_dist)); // Wall column size in pixels

                    // Where the column starts and ends relative to the screen.
                    float column_start = (screen.flt_height - 1 - columnHeight) / 2f;
                    float column_end = (screen.flt_height - 1 + columnHeight) / 2f;

                    // Wall rendering bounds on the screen...
                    int draw_column_start = screen.Height - (int)(screen.Height - column_start);    // Inclusive
                    int draw_column_end = screen.Height - (int)(screen.Height - column_end);        // Exclusive

                    // Which cannot exceed the full screen bounds.
                    if (draw_column_start < 0)
                        draw_column_start = 0;
                    if (draw_column_end > screen.Height)
                        draw_column_end = screen.Height;

                    // Draws the background before the wall.
                    // Critical performance impact.
                    if (scene->background.source.Buffer != IntPtr.Zero)
                        for (int line = 0; line < draw_column_start; line++)
                            drawBackground(line);

                    // Draw the wall
                    // Critical performance impact.
                    for (int line = draw_column_start; line < draw_column_end; line++)
                    {
                        float vratio = (line - column_start) / columnHeight;
                        Color color = nearest->texture.MapPixel(nearest_ratio, vratio);
                        screen[screen_column, line] = color;
                    }

                    // Draw the other side of the background
                    // Critical performance impact.
                    if (scene->background.source.Buffer != IntPtr.Zero)
                        for (int line = draw_column_end; line < screen.Height; line++)
                        {
                            //drawFloorOrBackground(line);
                        }
                    #endregion

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    void drawFloorOrBackground(int line)
                    {
                        float fall_dist = cache->fall_dists[line];

                        Vector camera_dir = new(scene->camera->rotation);
                        Vector center_floor_hit = scene->camera->position + camera_dir * fall_dist;
                        float scratio = screen.flt_width / screen.flt_height;
                        float factor = cache->fall_factors[line];
                        Vector lr_direction = new Vector(camera_dir.Y, -camera_dir.X) * scratio * factor;
                        Vector left_floor_hit = center_floor_hit - lr_direction * 0.5f;
                        float step = 1 / screen.flt_width;
                        Vector point = left_floor_hit + screen_column * step * lr_direction;

                        SFloor* strf = scene->FloorAt(point);
                        if (strf != null)
                        {
                            screen[screen_column, line] = strf->MapTexture(point);
                        }
                        else
                        {
                            drawBackground(line);
                        }
                    }

                    // Draws background
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    void drawBackground(int line)
                    {
                        float background_hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0
                        float screenVratio = line / screen.flt_height;
                        float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                        uint color = background.MapPixel(background_hratio, background_vratio);
                        screen[screen_column, line] = color;
                    }
                }
            }
        }

        private static List<Effect> postProcessing = new List<Effect>();
        private static void PostProcess(Image target)
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