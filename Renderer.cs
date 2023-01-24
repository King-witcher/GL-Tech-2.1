using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Engine.Imaging;
using Engine.Imaging.Processing;
using Engine.World;
using Engine.Scripting;
using Engine.Structs;
using System.Runtime.CompilerServices;

namespace Engine;

public sealed partial class Renderer
{
    unsafe RenderCache* cache;
    Image frontBuffer;
    Scene currentScene = null;
    Action ScheduledActions = null;
    public bool ParallelRendering { get; set; } = true;

    public Scene ActiveScene => currentScene;

    private float minframetime = 2;
    public int MaxFps
    {
        get => (int)(1000f / minframetime);
        set
        {
            Util.Clip(ref value, 1, 500);
            minframetime = 1000f / value;
        }
    }

    bool doubleBuffer = true;
    public bool SynchronizeThreads
    {
        get => doubleBuffer;
        set => ChangeIfNotRunning("DoubleBuffer", ref doubleBuffer, value);
    }

    private int customWidth = 960;
    public int CustomWidth
    {
        get => customWidth;
        set => ChangeIfNotRunning("CustomWidth", ref customWidth, value);
    }

    private int customHeight = 520;
    public int CustomHeight
    {
        get => customHeight;
        set => ChangeIfNotRunning("CustomHeight", ref customHeight, value);
    }

    bool fullScreen;
    public bool FullScreen
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

    float fieldOfView = 90f;
    public float FieldOfView
    {
        get => fieldOfView;
        set
        {
            Util.Clip(ref value, 1f, 179f);
            ChangeIfNotRunning("FieldOfView", ref fieldOfView, value);
        }
    }

    bool captureMouse = false;
    public bool CaptureMouse
    {
        get => captureMouse;
        set => captureMouse = value;    // Revisar
    }

    public bool IsRunning { get; private set; } = false;

    public Image GetScreenshot()
    {
        Image screenshot = new Image(CustomWidth, CustomHeight);
        Image.BufferCopy(frontBuffer, screenshot);
        return screenshot;
    }

    public void AddEffect(Effect effect)
    {
        postProcessing.Add(effect);
    }

    public void AddEffect<EffectClass>() where EffectClass : Effect, new()
    {
        AddEffect(new EffectClass());
    }

    public unsafe void Run(Scene scene)
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
        Canvas window = new(frontBuffer, FullScreen);

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
        var controlThread = Task.Run(() => SyncControlThread(frontBuffer, window, in stopRequested));

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

    private unsafe void RefreshCache()
    {
        if (cache != null)
            RenderCache.Delete(cache);
        cache = RenderCache.Create(CustomWidth, CustomHeight, FieldOfView);
    }

    private void Schedule(Action action) => ScheduledActions += action;

    // Executa ações que foram agendadas enquanto a engine renderizava.
    private void RunScheduled()
    {
        ScheduledActions?.Invoke();
        ScheduledActions = null;
    }

    internal unsafe void SyncControlThread(Image frontBuffer, Canvas canvas, in bool stopRequested)
    {
        // Spaguetti
        RefreshCache();

        // Buffer where the image will be rendered
        Image backBuffer = SynchronizeThreads ?
            new(frontBuffer.Width, frontBuffer.Height) :
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
        float framerate = 0f;

        canvas.RePaint += Render;

        void Render(TimeSpan time)
        {
            controlStopwatch.Restart();
            Script.Frame.BeginRender();

            Draw(backBuffer, currentScene.unmanaged);
            //DrawFloors(backBuffer, currentScene.unmanaged);
            //PostProcess(backBuffer);

            framerate = 0.95f * framerate + 0.05f / (float)Script.Frame.RenderTime;

            if (framerate == float.PositiveInfinity)
                framerate = 0f;

            GUI.Text text = new GUI.Text("");
            text.Value = ((int)framerate).ToString();
            text.Render(backBuffer);

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
    }

    private unsafe void ControlThread(Image frontBuffer, in bool stopRequested)
    {
        // Spaguetti
        RefreshCache();

        // Buffer where the image will be rendered
        Image backBuffer = SynchronizeThreads ?
            new(frontBuffer.Width, frontBuffer.Height) :
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

    private unsafe void Draw(Image screen, SceneStruct* scene)
    {
        ushort[] column_height_table = new ushort[screen.Width];

        // Cull only the planes that appear in the field of view.
        View view = new View(scene->camera->position, new(cache->angles[0] + scene->camera->rotation), new(cache->angles[screen.Width - 1] + scene->camera->rotation));
        using PlaneList plane_list = scene->plane_list.CullBySurface(scene->camera->position).CullByFrustum(view);

        // Checks if the code should be run in all cores or just one.
        {
            if (ParallelRendering)
                Parallel.For(fromInclusive: 0, toExclusive: screen.Width, body: DrawColumn);
            else
                for (int i = 0; i < screen.Width; i++)
                    DrawColumn(i);
        }

        // Checks if the code should be run in all cores or just one.
        if (ParallelRendering)
            Parallel.For(fromInclusive: 0, toExclusive: screen.Height >> 1, body: drawCeilingLine);
        else
            for (int i = 0; i < screen.Height >> 1; i++)
                drawCeilingLine(i);

        // Checks if the code should be run in all cores or just one.
        if (ParallelRendering)
            Parallel.For(fromInclusive: screen.Height >> 1, toExclusive: screen.Height, body: drawFloorLine);
        else
            for (int i = screen.Height >> 1; i < screen.Height; i++)
                drawFloorLine(i);

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawFloorLine(int line)
        {
            unchecked
            {
                float fall_dist = cache->fall_dists[line - (screen.Height >> 1)];

                Vector camera_dir = new(scene->camera->rotation);

                Vector center_floor_hit = scene->camera->position + camera_dir * fall_dist;
                float scratio = screen.flt_width / screen.flt_height;
                float factor = cache->fall_factors[line - (screen.Height >> 1)];
                Vector lr_direction = new Vector(camera_dir.Y, -camera_dir.X) * scratio * factor;
                Vector left_floor_hit = center_floor_hit - lr_direction * 0.5f;

                using HorizontalList list = scene->floor_list.GetIntersections(left_floor_hit, left_floor_hit + lr_direction);

                for (ushort screen_column = 0; screen_column < screen.Width; screen_column++)
                {
                    if ((column_height_table[screen_column] + screen.Height) >> 1 > line)
                        continue;
                    Vector point = left_floor_hit + screen_column * lr_direction / screen.flt_width;

                    HorizontalStruct* strf = list.Locate(point);
                    if (strf != null)
                    {
                        screen[screen_column, line] = strf->MapTexture(point);
                    }
                    else
                    {
                        screen[screen_column, line] = 0;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawCeilingLine(int line)
        {
            unchecked
            {
                float fall_dist = cache->fall_dists[(screen.Height >> 1) - line - 1];

                Vector camera_dir = new(scene->camera->rotation);

                Vector center_floor_hit = scene->camera->position + camera_dir * fall_dist;
                float scratio = screen.flt_width / screen.flt_height;
                float factor = cache->fall_factors[(screen.Height >> 1) - line - 1];
                Vector lr_direction = new Vector(camera_dir.Y, -camera_dir.X) * scratio * factor;
                Vector left_floor_hit = center_floor_hit - lr_direction * 0.5f;

                using HorizontalList list = scene->ceiling_list.GetIntersections(left_floor_hit, left_floor_hit + lr_direction);

                for (ushort screen_column = 0; screen_column < screen.Width; screen_column++)
                {
                    if ((screen.Height - column_height_table[screen_column]) >> 1 < line + 1)
                        continue;
                    Vector point = left_floor_hit + screen_column * lr_direction / screen.flt_width;

                    HorizontalStruct* strf = list.Locate(point);
                    if (strf != null)
                    {
                        screen[screen_column, line] = strf->MapTexture(point);
                    }
                    else
                    {
                        screen[screen_column, line] = 0;
                    }
                }
            }
        }

        // Render a vertical column of the screen.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void DrawColumn(int screen_column)
        {
            unchecked
            {
                // Caching frequently used variables
                float ray_cos = cache->cosines[screen_column];
                float ray_angle = cache->angles[screen_column] + scene->camera->rotation;
                Texture background = scene->background;
                Segment ray = new Segment(scene->camera->position, ray_angle);

                // Cast the ray towards every plane.
                PlaneStruct* nearest = plane_list.NearestPlane(ray, out float nearest_dist, out float nearest_ratio);

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

                column_height_table[screen_column] = (ushort)(draw_column_end - draw_column_start);

                // Draws the background before the wall.
                // Critical performance impact.
                //if (scene->background.source.Buffer != IntPtr.Zero)
                //    for (int line = 0; line < draw_column_start; line++)
                //        drawBackground(line);

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
                //if (scene->background.source.Buffer != IntPtr.Zero)
                //    for (int line = draw_column_end; line < screen.Height; line++)
                //    {
                        //drawFloorOrBackground(line);
                //    }
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

                    HorizontalStruct* strf = scene->FloorAt(point);
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
                [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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

    private List<Effect> postProcessing = new List<Effect>();
    private void PostProcess(Image target)
    {
        foreach (var effect in postProcessing)
            effect.Process(target);
    }

    void ChangeIfNotRunning<T>(string name, ref T obj, T value)
    {
        if (IsRunning)
            Debug.InternalLog(
                message: $"The value of \"{name}\" cannot be modified while running. Value will keep \"{obj}\".",
                debugOption: Debug.Options.Warning);
        else
            obj = value;
    }
}