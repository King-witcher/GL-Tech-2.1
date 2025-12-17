using GLTech.Input;
using GLTech.Scripting;
using GLTech.World;
using System;
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
    private static Logger logger = new("Engine");

    public bool ParallelRendering { get; set; }
    public bool FullScreen { get; set; }
    public float FieldOfView { get; set; }
    public bool CaptureMouse { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool IsRunning { get; private set; }

    public Engine(EngineCreateInfo createInfo)
    {
        window = new Window(createInfo.Title, createInfo.WindowWidth, createInfo.WindowHeight, createInfo.FullScreen);
        ParallelRendering = createInfo.ParallelRendering;
        FullScreen = createInfo.FullScreen;
        FieldOfView = createInfo.FieldOfView;
        CaptureMouse = createInfo.CaptureMouse;
        WindowWidth = createInfo.WindowWidth;
        WindowHeight = createInfo.WindowHeight;
    }

    public Engine() : this(new EngineCreateInfo()) { }

    public unsafe void Run(Scene scene)
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

        bool quitRequested = false;

        var controlStopwatch = new Stopwatch();   // Required to cap framerate
        // Run setup scripts
        Script.Time.RenderTime = 0;
        Script.Time.TimeStep = 0;
        Script.Time.WindowTime = 0;
        currentScene.Start?.Invoke();

        Renderer renderer = new(window.Buffer);

        window.RelativeMouseMode = CaptureMouse;

        window.OnQuit += () => { quitRequested = true; };
        window.OnKeyDown += Keyboard.SetKeyDown;
        window.OnKeyUp += Keyboard.SetKeyUp;

        long FIXED_TIMESTEP = Stopwatch.Frequency / Script.Time.FIXED_TICKS_PER_SECOND;

        long initTime = Stopwatch.GetTimestamp();
        long lastTime = initTime;
        long accumulator = 0;
        while (!Scripting.Input.ShouldExit)
        {
            // Draw current state
            renderer.Draw(currentScene.unmanaged);
            window.Present();

            // Update timers
            long newTime = Stopwatch.GetTimestamp();
            long frameTime = newTime - lastTime;
            lastTime = newTime;
            accumulator += frameTime;

            // Update input state
            Scripting.Input.Update();
            FlushSchedule();
            window.ProcessEvents();
            //if (CaptureMouse)
            //    Mouse.Shift = window.GetMouseShift();

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
        Scripting.Input.ShouldExit = false;

        window.Destroy();

        // Clears the Keyboard
        Keyboard.Clear();

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