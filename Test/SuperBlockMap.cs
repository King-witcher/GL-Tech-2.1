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
        public class SuperBlockMap : Scene
        {
            Image crazyBackground_buffer = new(DemoTextures.CrazyBackground);
            Image blockmap_buffer = new(DemoTextures.Plant);
            Image test_buffer = new(DemoTextures.Test);

            public SuperBlockMap()
            {
                Texture background = new Texture(crazyBackground_buffer, 0, 3);
                this.Background = background;

                // BlockMap
                {
                    Texture tex = new(test_buffer, 0f, 1f, 0f, 25f);
                    BlockMap.TextureMapper tb = new();
                    tb[(0, 0, 0)] = tex;
                    BlockMap gm = new(blockmap_buffer, tb);
                    gm.WorldScale = 0.04f;
                    Add(gm);
                }

                // Camera
                {
                    Camera camera = Camera;
                    camera.AddScript(new MouseLook(2.2f));
                    camera.AddScript<DebugPerformanceStats>();
                    camera.AddScript<DebugSceneInfo>();

                    // Tratador de colisão
                    PointCollider pointCollider = new();
                    SoftMovement softMovement = new(pointCollider);
                    camera.AddScript(pointCollider);
                    camera.AddScript(softMovement);

                    Add(camera);
                }

                Engine.Renderer.CaptureMouse = true;
                Engine.Renderer.FullScreen = true;
                Engine.Renderer.SynchronizeThreads = true;
                // Engine.Renderer.Run(this);
            }

            protected override void Delete()
            {
                blockmap_buffer.Dispose();
                test_buffer.Dispose();
                crazyBackground_buffer.Dispose();
            }
        }
    }
}
