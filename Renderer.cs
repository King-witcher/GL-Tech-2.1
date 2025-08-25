using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Engine.Imaging;
using Engine.Imaging.Processing;
using Engine.World;
using Engine.Scripting;
using Engine.Structs;
using System.Runtime.CompilerServices;

using Engine.Input;

namespace Engine;

public static partial class Renderer
{
    unsafe static RenderCache* cache;
    static Image frontBuffer;
    static Scene currentScene = null;
    static Action ScheduledActions = null;
    private static Logger logger = new("Renderer");
    public static bool ParallelRendering { get; set; } = true;

    public static Scene ActiveScene => currentScene;

    private static float minframetime = 2;
    public static int MaxFps
    {
        get => (int)(1000f / minframetime);
        set
        {
            Util.Clip(ref value, 1, 500);
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
                CustomWidth = 1920; // Gambiarra
                customHeight = 1080;
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
        postProcessing.Add(effect);
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
            logger.Error($"Cannot render a null Scene.");
            return;
        }

        if (scene.Background.source.Buffer == IntPtr.Zero)
            logger.Warn($"The Scene being rendered does not have a background texture. Add it by using Scene.Background property.");
        #endregion

        Camera camera = scene.Camera;
        currentScene = scene;

        // Unmanaged buffer where the video will be put.
        frontBuffer = new(CustomWidth, CustomHeight);

        // Spaguetti
        RefreshCache();

        bool quitRequested = false;

        // Buffer where the image will be rendered
        Image backBuffer = SynchronizeThreads ?
            new(frontBuffer.Width, frontBuffer.Height) :
            frontBuffer;

        #region Warnings
        if (!SynchronizeThreads && postProcessing.Count > 0)
            logger.Warn("The renderer has post processing effects set but DoubleBuffering is disabled. Post processing effects may not work properly.");
        #endregion

        Stopwatch controlStopwatch = new Stopwatch();   // Required to cap framerate
        Script.Frame.RestartFrame();
        Script.Frame.BeginScript();
        currentScene.Start?.Invoke();
        Script.Frame.EndScript();
        float framerate = 0f;

        Stopwatch sw = new Stopwatch();


        Window window = new Window(frontBuffer, fullScreen);
        window.OnQuit += () => { quitRequested = true; };
        window.OnKeyDown += Keyboard.SetKeyDown;
        window.OnKeyUp += Keyboard.SetKeyUp;
        window.Spawn();
        while (!quitRequested)
        {
            sw.Restart();
            window.Update();
            sw.Stop();
            TimeSpan time = sw.Elapsed;

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

            //while (controlStopwatch.ElapsedMilliseconds < minframetime)
            //    Thread.Yield();

            FlushSchedule();
            Script.Frame.RestartFrame();
            Script.Frame.BeginScript();
            window.PollEvents();
            if (CaptureMouse)
                Mouse.Shift = window.GetMouseShift();
            currentScene.OnFrame?.Invoke();
            Script.Frame.EndScript();
        }

        window.Close();

        // Clears the Keyboard
        Input.Keyboard.Clear();

        // Finally, dispose everythihng.
        //window.Dispose();
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
    private static void FlushSchedule()
    {
        ScheduledActions?.Invoke();
        ScheduledActions = null;
    }

    private unsafe static void Draw(Image screen, SceneStruct* scene)
    {
        var col_start_table = new int[screen.Width];
        var col_end_table = new int[screen.Width];

        // Cull only the planes that appear in the field of view.
        View view = new View(scene->camera->position, new(cache->angles[0] + scene->camera->rotation), new(cache->angles[screen.Width - 1] + scene->camera->rotation));
        using var surface_culled = scene->plane_list.CullBySurface(scene->camera->position);
        using var plane_list = surface_culled.CullByFrustum(view);

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
                float hit_dist = scene->camera->z / ((line - screen.flt_height * 0.5f) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = new(scene->camera->rotation);
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (screen.Width >> 1);
                using HorizontalList list = scene->floor_list.GetIntersections(left_bound_hit, left_bound_hit + right);
                var step_vec = right * step;
                for (int col = 0; col < screen.Width; col++)
                {
                    if (col_end_table[col] > line) continue;

                    Vector floor_hit = left_bound_hit + col * step_vec;
                    HorizontalStruct* floor = list.Locate(floor_hit);

                    if (floor != null)
                        screen[col, line] = floor->MapTexture(floor_hit);
                    else
                        screen[col, line] = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawCeilingLine(int line)
        {
            unchecked
            {
                float hit_dist = (1 - scene->camera->z) / ((screen.flt_height * 0.5f - line) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = new(scene->camera->rotation);
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (screen.Width >> 1);
                using HorizontalList list = scene->floor_list.GetIntersections(left_bound_hit, left_bound_hit + right);

                var step_vec = right * step;
                for (int col = 0; col < screen.Width; col++)
                {
                    if (col_start_table[col] <= line) continue;

                    Vector hit = left_bound_hit + step_vec * col;
                    HorizontalStruct* ceiling = list.Locate(hit);

                    if (ceiling != null)
                        screen[col, line] = ceiling->MapTexture(hit);
                    else
                        screen[col, line] = 0;
                }
            }
        }

        // Render a vertical column of the screen.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void DrawColumn(int scr_col)
        {
            unchecked
            {
                // Caching frequently used variables
                float ray_cos = cache->cosines[scr_col];
                float ray_angle = cache->angles[scr_col] + scene->camera->rotation;
                Texture bg = scene->background;
                Segment ray = new Segment(scene->camera->position, ray_angle);

                // Cast the ray towards every plane.
                PlaneStruct* plane = plane_list.NearestPlane(ray, out float col_dist, out float plane_ratio);

                // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                #region Render the plane

                // Height that the current column should have on the screen.
                float col_h = (cache->colHeight1 / (ray_cos * col_dist)); // Wall column size in pixels

                // Where the column starts and ends relative to the screen.
                float col_start = (screen.flt_height - 1f - col_h) * 0.5f + col_h * (scene->camera->z - 0.5f);
                float col_end = (screen.flt_height - 1f + col_h) * 0.5f + col_h * (scene->camera->z - 0.5f);

                // Wall rendering bounds on the screen...
                int draw_col_start = screen.Height - (int)(screen.flt_height - col_start);    // Inclusive
                int draw_col_end = screen.Height - (int)(screen.flt_height - col_end);        // Exclusive

                // Which cannot exceed the full screen bounds.
                if (draw_col_start < 0)
                    draw_col_start = 0;
                if (draw_col_end > screen.Height)
                    draw_col_end = screen.Height;

                col_start_table[scr_col] = draw_col_start;
                col_end_table[scr_col] = draw_col_end;

                // Draw the wall
                // Critical performance impact.
                float i_col_h = 1f / col_h;
                for (int line = draw_col_start; line < draw_col_end; line++)
                {
                    float vratio = (line - col_start) * i_col_h;
                    Color c = plane->texture.MapPixel(plane_ratio, vratio);
                    screen[scr_col, line] = c;
                }
                #endregion
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
            logger.Warn($"The value of \"{name}\" cannot be modified while running. Value will keep \"{obj}\".");
        else
            obj = value;
    }
}