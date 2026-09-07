using Engine.Window;
using GLTech.Scripting;
using GLTech.World;
using System.Diagnostics;

namespace GLTech;

public struct GameEngineCreateInfo
{
    //public int WindowWidth = 800;
    //public int WindowHeight = 600;
    //public bool ParallelRendering = true;
    //public float FieldOfView = 90f;
    public int MaxFPS = int.MaxValue;
    //public bool FullScreen = false;
    //public bool CaptureMouse = true;
    //public string Title = "GL Tech Window";
    public string FsBasePath = "base";

    public GameEngineCreateInfo() { }

}

public class GameEngine
{
    Scene? currentScene = null;
    Action? ScheduledActions = null;
    //LegacyWindow window;
    Renderer renderer;
    private static Logger logger = new("Engine");

    //public bool ParallelRendering
    //{
    //    get => renderer.ParallelRendering;
    //    set => renderer.ParallelRendering = value;
    //}
    //public bool FullScreen
    //{
    //    get => window.Fullscreen;
    //    set => window.Fullscreen = value;
    //}
    //public float FieldOfView
    //{
    //    get => renderer.HFov;
    //    set => renderer.HFov = value;
    //}
    //public bool CaptureMouse
    //{
    //    get => window.RelativeMouseMode;
    //    set => window.RelativeMouseMode = value;
    //}
    //public (int width, int height) WindowSize
    //{
    //    get => window.Size;
    //    set => window.Size = value;
    //}
    public bool IsRunning { get; private set; }

    public GameEngine(GameEngineCreateInfo createInfo)
    {
        //window = new LegacyWindow(
        //    title: createInfo.Title,
        //    width: createInfo.WindowWidth,
        //    height: createInfo.WindowHeight,
        //    fullscreen: createInfo.FullScreen,
        //    vsync: false);
        //renderer = new Renderer(window.Buffer);

        //ParallelRendering = createInfo.ParallelRendering;
        //FieldOfView = createInfo.FieldOfView;
        //CaptureMouse = createInfo.CaptureMouse;
    }

    public GameEngine() : this(new GameEngineCreateInfo()) { }

    public unsafe void Run(IWindow window, Renderer renderer, Scene scene)
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

        Player camera = scene.Player;
        currentScene = scene;

        bool quitRequested = false;

        var controlStopwatch = new Stopwatch();   // Required to cap framerate
        // Run setup scripts
        Script.Time.RenderTime = 0;
        Script.Time.TimeStep = 0;
        currentScene.Start();

        //window.RelativeMouseMode = CaptureMouse;

        //window.OnQuit += () => { quitRequested = true; };

        long FIXED_TIMESTEP = Stopwatch.Frequency / Script.Time.FixedTicks;

        var sw = new Stopwatch();
        long initTime = Stopwatch.GetTimestamp();
        long lastTime = initTime;
        long accumulator = 0;

        while (!Script.Input.ShouldExit && !quitRequested)
        {
            var target = window.AcquireImage();
            // Draw current state
            sw.Restart();
            renderer.Render(currentScene, target);
            sw.Stop();
            Debug.RenderTime = (float)sw.Elapsed.TotalSeconds;

            sw.Restart();
            window.Present(target);
            sw.Stop();
            Debug.PresentTime = (float)sw.Elapsed.TotalSeconds;
            Debug.ClearLogs();

            // Update timers
            long currentTime = Stopwatch.GetTimestamp();
            long totalTime = currentTime - initTime;
            long currentStep = currentTime - lastTime;
            accumulator += currentStep;

            // Update input state
            Script.Input.Update();
            FlushSchedule();

            // Run fixed updates
            sw.Restart();
            Script.Time.TimeStep = (float)FIXED_TIMESTEP / Stopwatch.Frequency;
            Script.Time.FixedRemainder = 0f;
            while (accumulator >= FIXED_TIMESTEP)
            {
                Script.Time.TotalTime = (float)(totalTime - accumulator + FIXED_TIMESTEP) / Stopwatch.Frequency;
                currentScene.FixedUpdate(Script.Time.TimeStep);
                accumulator -= FIXED_TIMESTEP;
            }

            // Run frame update
            Script.Time.TimeStep = (float)currentStep / Stopwatch.Frequency;
            Script.Time.TotalTime = (float)totalTime / Stopwatch.Frequency;
            Script.Time.FixedRemainder = (float)accumulator / FIXED_TIMESTEP;
            currentScene.Update(Script.Time.TimeStep);
            sw.Stop();
            Debug.ScriptTime = (float)sw.Elapsed.TotalSeconds;

            lastTime = currentTime;
        }
        window.Close();

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
