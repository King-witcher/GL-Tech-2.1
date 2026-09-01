using Engine;
using GLTech.Scripting.Debugging;
using GLTech.Scripting.Physics;
using GLTech.Scripting.Prefab;
using GLTech.World;
using GLTech.World.Composed;

namespace GLTech.Demos.SuperBlockMap
{
    // Esse é um mapa com uma quantidade extrema de planos para testes de gargalo.
    public class Map : Scene
    {
        TextureBuffer plant = ResourceManager.LoadTextureBuffer("plant.png");
        TextureBuffer test_texture = ResourceManager.LoadTextureBuffer("test.png");

        protected override void Delete()
        {
            plant.Dispose();
            test_texture.Dispose();
        }

        public Map()
        {
            // BlockMap
            {
                Texture tex = new(test_texture, 0f, 1f, 0f, 25f);

                Dictionary<Color, Texture> dict = new();
                dict[(255, 0, 0)] = tex;

                BlockMap blockMap = new(
                    map: plant,
                    mapTexture: color =>
                    {
                        if (dict.TryGetValue(color, out Texture texture))
                            return texture;
                        else return null;
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
                Player.AddScript(new MouseLook(2.2f));
                Player.AddScript<DebugPerformance>();
                // Camera.AddScript<DebugScene>();

                // Tratador de colisão
                KinematicBody body = new();
                Q1Controller softMovement = new();
                Player.AddScript(body);
                Player.AddScript(softMovement);
            }
        }
    }
}
