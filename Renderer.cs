using Engine.Imaging;
using Engine.Imaging.Processing;
using Engine.Input;
using Engine.Scripting;
using Engine.Structs;
using Engine.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Engine;

public static partial class Renderer
{
    unsafe static RenderCache* cache;
    static Image frameBuffer;
    static Scene? currentScene = null;
    static Action? ScheduledActions = null;
    static Window? window;
    private static Logger logger = new("Renderer");
    public static bool ParallelRendering { get; set; } = true;

    public static Scene? ActiveScene => currentScene;

    private static float minframetime = 1;
    public static int MaxFps
    {
        get => (int)(1000f / minframetime);
        set
        {
            Util.Clip(ref value, 1, 1000);
            minframetime = 1000f / value;
        }
    }

    private static int initialWidth = 960;
    public static int CustomWidth
    {
        get => initialWidth;
        set => ChangeIfNotRunning("CustomWidth", ref initialWidth, value);
    }

    private static int customHeight = 540;
    public static int CustomHeight
    {
        get => customHeight;
        set => ChangeIfNotRunning("CustomHeight", ref customHeight, value);
    }

    static bool initialFullscreen;
    public static bool FullScreen
    {
        get
        {
            if (window != null)
                return window.Fullscreen;
            return initialFullscreen;
        }
        set
        {
            initialFullscreen = value;
            if (window != null)
                window.Fullscreen = value;
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

    public static bool CaptureMouse
    {
        get => Window.CaptureMouse;
        set => Window.CaptureMouse = value;
    }

    public static bool IsRunning { get; private set; } = false;

    public static Image GetScreenshot()
    {
        Image screenshot = new Image(CustomWidth, CustomHeight);
        Image.BufferCopy(frameBuffer, screenshot);
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

        // Spaguetti
        RefreshCache();

        bool quitRequested = false;

        var controlStopwatch = new Stopwatch();   // Required to cap framerate
        // Run setup scripts
        Script.Time.RenderTime = 0;
        Script.Time.TimeStep = 0;
        Script.Time.WindowTime = 0;
        currentScene.Start?.Invoke();

        window = new(
            title: "GL Tech 2.1",
            w: CustomWidth,
            h: customHeight,
            bufw: CustomWidth,
            bufh: customHeight,
            fullscreen: FullScreen,
            out frameBuffer
        );

        window.OnQuit += () => { quitRequested = true; };
        window.OnKeyDown += Keyboard.SetKeyDown;
        window.OnKeyUp += Keyboard.SetKeyUp;

        long FIXED_TIMESTEP = Stopwatch.Frequency / Script.Time.FIXED_TICKS_PER_SECOND;

        long initTime = Stopwatch.GetTimestamp();
        long lastTime = initTime;
        long accumulator = 0;
        while (!quitRequested)
        {
            // Draw current state
            Draw(frameBuffer, currentScene.unmanaged);
            window.Present();

            // Update timers
            long newTime = Stopwatch.GetTimestamp();
            long frameTime = newTime - lastTime;
            lastTime = newTime;
            accumulator += frameTime;

            // Update input state
            FlushSchedule();
            window.ProcessEvents();
            if (CaptureMouse)
                Mouse.Shift = window.GetMouseShift();

            // Run fixed ticks
            Script.Time.TimeStep = (float)FIXED_TIMESTEP / Stopwatch.Frequency;
            Script.Time.FixedRemainder = 0f;
            while (accumulator >= FIXED_TIMESTEP)
            {
                currentScene.OnFixedTick?.Invoke();
                accumulator -= FIXED_TIMESTEP;
            }

            //// Run per frame tick
            Script.Time.TimeStep = (float)frameTime / Stopwatch.Frequency;
            Script.Time.FixedRemainder = (float)accumulator / FIXED_TIMESTEP;
            currentScene.OnFrame?.Invoke();
        }

        window.Destroy();

        // Clears the Keyboard
        Keyboard.Clear();

        currentScene = null;
        IsRunning = false;
    }

    private static unsafe void RefreshCache()
    {
        if (cache != null)
            RenderCache.Delete(cache);
        cache = RenderCache.Create(FieldOfView, CustomWidth);
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
        var col_start_table = new int[screen.width];
        var col_end_table = new int[screen.width];

        // Cull only the planes that appear in the field of view.
        var left = new Vector(-scene->camera->direction.y, scene->camera->direction.x);
        var rtl = left * cache->step0 * screen.widthf;
        View view = new(
            center: scene->camera->position,
            left: scene->camera->direction + rtl * 0.5f,
            right: scene->camera->direction - rtl * 0.5f
        );
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
                float hit_dist = scene->camera->z / ((line - screen.heightf * 0.5f) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = scene->camera->direction;
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (screen.Width >> 1);
                using HorizontalList list = scene->floor_list.GetIntersections(left_bound_hit, left_bound_hit + right);
                var step_vec = right * step;
                for (int col = 0; col < screen.Width; col++)
                {
                    if (col_end_table[col] > line) continue;

                    Vector floor_hit = left_bound_hit + col * step_vec;
                    HorizontalStruct* floor = list.FindAndRaise(floor_hit);

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
                float hit_dist = (1 - scene->camera->z) / ((screen.heightf * 0.5f - line) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = scene->camera->direction;
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (screen.Width >> 1);
                using HorizontalList list = scene->ceiling_list.GetIntersections(left_bound_hit, left_bound_hit + right);

                var step_vec = right * step;
                for (int col = 0; col < screen.Width; col++)
                {
                    if (col_start_table[col] <= line) continue;

                    Vector hit = left_bound_hit + step_vec * col;
                    HorizontalStruct* ceiling = list.FindAndRaise(hit);

                    if (ceiling != null)
                        screen[col, line] = ceiling->MapTexture(hit);
                    else
                        screen[col, line] = 0;
                }
            }
        }

        // Render a vertical column of the screen.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void DrawColumn(int col_idx)
        {
            unchecked
            {
                int width = screen.width;
                int height = screen.height;
                float height_f = screen.heightf;

                // Caching frequently used variables
                var delta = width / 2 - col_idx;
                var dir = scene->camera->direction + left * (cache->step0 * delta);
                Segment ray = new Segment(scene->camera->position, dir);
                //Texture bg = scene->background;

                // Cast the ray towards every plane.
                PlaneStruct* plane = plane_list.NearestPlane(ray, out Vector rs);
                //if (plane == null) return;

                // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                #region Render the plane

                // Height that the current column should have on the screen.
                float colision_depth = Vector.DotProduct(ray.direction, scene->camera->direction) * rs.x;
                float col_height_f = cache->colHeight1 / colision_depth; // Wall column size in pixels

                // Where the column starts and ends relative to the screen.
                float col_start_f = (height_f - 1f - col_height_f) * 0.5f + col_height_f * (scene->camera->z - 0.5f);
                float col_end_f = (height_f - 1f + col_height_f) * 0.5f + col_height_f * (scene->camera->z - 0.5f);

                // Wall rendering bounds on the screen...
                int col_start_i = height - (int)(height_f - col_start_f);    // Inclusive
                if (col_start_i < 0)
                    col_start_i = 0;

                int col_end_i = height - (int)(height_f - col_end_f);        // Exclusive
                if (col_end_i > height)
                    col_end_i = height;

                col_start_table[col_idx] = col_start_i;
                col_end_table[col_idx] = col_end_i;

                // Draw the wall
                // Critical performance impact.
                float i_col_h = 1f / col_height_f;
                for (int line_idx = col_start_i; line_idx < col_end_i; line_idx++)
                {
                    float v = (line_idx - col_start_f) * i_col_h;
                    Color color = plane->texture.MapNearest(rs.y, v);
                    screen[col_idx, line_idx] = color;
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