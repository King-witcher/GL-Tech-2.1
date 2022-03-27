using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Test;

namespace Test
{
    partial class Program
    {
        // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
        public class ExtremeGrid : Scene
        {
            Image BlockMapbuf = new(DemoTextures.Plant);
            Image Test = new(DemoTextures.Test);
            Image backgroundbuf = new(DemoTextures.CrazyBackground);


            public ExtremeGrid()
            {
                Texture background = new Texture(backgroundbuf, 0, 3);
                this.Background = background;

                // BlockMap
                {
                    Texture tex = new(Test, 0f, 1f, 0f, 25f);
                    BlockMap.TextureMapper tb = new();
                    tb[(0, 0, 0)] = tex;
                    BlockMap gm = new(BlockMapbuf, tb);
                    gm.WorldScale = 0.04f;
                    Add(gm);
                }

                // Camera
                {
                    Camera camera = Camera;
                    camera.AddScript(new MouseLook(2.2f));
                    camera.AddScript<DebugPerformanceStats>();
                    camera.AddScript<DebugSceneInfo>();

                    KinematicBody kinematicBody = new PointCollider();
                    SoftMovement softMovement = new SoftMovement(kinematicBody);
                    camera.AddScript(kinematicBody);
                    camera.AddScript(softMovement);

                    Add(camera);
                }

                Engine.Renderer.CaptureMouse = true;
                Engine.Renderer.FullScreen = true;
                Engine.Renderer.SynchronizeThreads = true;
                // Engine.Renderer.Run(this);
            }
        }
    }
}
