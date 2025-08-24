using Engine.Imaging;
using Engine.Scripting.Debugging;
using Engine.Scripting.Physics;
using Engine.Scripting.Prefab;
using Engine.World;
using Engine.World.Composed;
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
            Background = background;

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
                    colliders: false
                );

                blockMap.WorldScale = 0.04f;
                Add(blockMap);
            }

            // Camera
            {
                Camera.AddScript(new MouseLook(2.2f));
                Camera.AddScript<DebugPerformance>();
                Camera.AddScript<DebugScene>();

                // Tratador de colisão
                PointCollider pointCollider = new();
                SoftMovement softMovement = new(pointCollider);
                Camera.AddScript(pointCollider);
                Camera.AddScript(softMovement);
            }
        }
    }
}
