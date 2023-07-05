using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Demos;

using System.Collections.Generic;

namespace Engine.Demos.SuperBlockMap
{
    // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
    public class Map : Scene
    {
        Image crazyBackground_buffer = new(Resources.GOT);
        Image blockmap_buffer = new(Resources.Plant);
        Image test_buffer = new(Resources.Test);

        protected override void Delete()
        {
            blockmap_buffer.Dispose();
            test_buffer.Dispose();
            crazyBackground_buffer.Dispose();
        }

        public Map()
        {
            Texture background = new Texture(crazyBackground_buffer, 0, 9, 0, 3);
            this.Background = background;

            // BlockMap
            {
                Texture tex = new(test_buffer, 0f, 1f, 0f, 25f);

                Dictionary<Color, Texture> dict = new();
                dict[(0, 0, 0)] = tex;

                BlockMap blockMap = new(
                    map: blockmap_buffer,
                    mapTexture: color =>
                    {
                        if (dict.TryGetValue(color, out Texture texture))
                            return texture;
                        else return Texture.NullTexture;
                    },
                    textureFilling: BlockMap.TextureFilling.Side,
                    optimize: true,
                    colliders: false);

                blockMap.WorldScale = 0.04f;
                Add(blockMap);
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

            global::Engine.Renderer.CaptureMouse = true;
            global::Engine.Renderer.FullScreen = true;
            global::Engine.Renderer.SynchronizeThreads = true;
            // Engine.Renderer.Run(this);
        }
    }
}
