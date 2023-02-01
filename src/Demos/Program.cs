using Engine.World;

namespace Engine.Demos
{
    unsafe static internal partial class Program
    {
        static Renderer renderer = new Renderer();

        static void Main()
        {
            SetupRenderer();

            Play<Wolfenstein.Map>();
            Play<SuperBlockMap.Map>();
        }

        static void SetupRenderer()
        {
            Debug.OpenConsole();
            renderer = new Renderer();
            renderer.CustomHeight = 600;
            renderer.CustomWidth = 800;
            renderer.FieldOfView = 72f;
            renderer.FullScreen = true;
            renderer.SynchronizeThreads = true;
            renderer.CaptureMouse = true;
        }

        static void Play<Map>() where Map : Scene, new()
        {
            using Map map = new();
            renderer.Play(map);
        }
    }
}
