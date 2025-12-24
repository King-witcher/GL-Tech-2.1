using GLTech.Scripting;
using GLTech.World;
using System.Diagnostics;

namespace GLTech;

public struct EngineCreateInfo
{
    public int WindowWidth = 800;
    public int WindowHeight = 600;
    public bool ParallelRendering = true;
    public float FieldOfView = 90f;
    public int MaxFPS = int.MaxValue;
    public bool FullScreen = false;
    public bool CaptureMouse = true;
    public string Title = "GL Tech Window";

    public EngineCreateInfo() { }

}

public class Engine
{
    Scene? currentScene = null;
    Action? ScheduledActions = null;
    Window window;
    Renderer renderer;
    private static Logger logger = new("Engine");

    public bool ParallelRendering
    {
        get => renderer.ParallelRendering;
        set => renderer.ParallelRendering = value;
    }
    public bool FullScreen
    {
        get => window.Fullscreen;
        set => window.Fullscreen = value;
    }
    public float FieldOfView
    {
        get => renderer.HFov;
        set => renderer.HFov = value;
    }
    public bool CaptureMouse
    {
        get => window.RelativeMouseMode;
        set => window.RelativeMouseMode = value;
    }
    public (int width, int height) WindowSize
    {
        get => window.Size;
        set => window.Size = value;
    }
    public bool IsRunning { get; private set; }

    public Engine(EngineCreateInfo createInfo)
    {
        window = new Window(createInfo.Title, createInfo.WindowWidth, createInfo.WindowHeight, createInfo.FullScreen);
        window.Position = (-1000, 300);
        renderer = new Renderer(window.Buffer);

        ParallelRendering = createInfo.ParallelRendering;
        FieldOfView = createInfo.FieldOfView;
        CaptureMouse = createInfo.CaptureMouse;
    }

    public Engine() : this(new EngineCreateInfo()) { }

    public unsafe void Run(Scene scene)
    {
        #region Checks
        if (IsRunning)
        {
            logger.Error($"Engine is already running a Scene. Cannot run multiple Scenes at once.");
            return;
        }
        IsRunning = true;

        if (scene == null)
        {
            logger.Error($"Cannot render a null Scene.");
            return;
        }

        //if (scene.Background.source.Buffer == IntPtr.Zero)
        //    logger.Warn($"The Scene being rendered does not have a background texture. Add it by using Scene.Background property.");
        #endregion

        Camera camera = scene.Camera;
        currentScene = scene;

        bool quitRequested = false;

        var controlStopwatch = new Stopwatch();   // Required to cap framerate
        // Run setup scripts
        Script.Time.RenderTime = 0;
        Script.Time.TimeStep = 0;
        Script.Time.WindowTime = 0;
        currentScene.Start();

        window.RelativeMouseMode = CaptureMouse;

        window.OnQuit += () => { quitRequested = true; };

        long FIXED_TIMESTEP = Stopwatch.Frequency / Script.Time.FixedTicks;

        long initTime = Stopwatch.GetTimestamp();
        long lastTime = initTime;
        long accumulator = 0;

        while (!Script.Input.ShouldExit && !quitRequested)
        {
            // Draw current state
            renderer.Render(currentScene.raw);
            window.Present();

            // Update timers
            long newTime = Stopwatch.GetTimestamp();
            long frameTime = newTime - lastTime;
            lastTime = newTime;
            accumulator += frameTime;

            // Update input state
            Script.Input.Update();
            FlushSchedule();
            //if (CaptureMouse)
            //    Mouse.Shift = window.GetMouseShift();

            // Run fixed ticks
            Script.Time.TimeStep = (float)FIXED_TIMESTEP / Stopwatch.Frequency;
            Script.Time.FixedRemainder = 0f;
            while (accumulator >= FIXED_TIMESTEP)
            {
                currentScene.FixedUpdate(Script.Time.TimeStep);
                accumulator -= FIXED_TIMESTEP;
            }

            //// Run per frame tick
            Script.Time.TimeStep = (float)frameTime / Stopwatch.Frequency;
            Script.Time.FixedRemainder = (float)accumulator / FIXED_TIMESTEP;
            currentScene.Update(Script.Time.TimeStep);
        }
        window.Destroy();

        // Clears the Keyboard
        Script.Input.Clear();

        currentScene = null;
        IsRunning = false;
    }

    // Executa ações que foram agendadas enquanto a engine renderizava.
    private void FlushSchedule()
    {
        ScheduledActions?.Invoke();
        ScheduledActions = null;
    }
}