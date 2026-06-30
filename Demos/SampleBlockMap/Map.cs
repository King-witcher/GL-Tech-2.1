using GLTech.Scripting.Debugging;
using GLTech.Scripting.Physics;
using GLTech.Scripting.Prefab;
using GLTech.World;
using GLTech.World.Composed;

namespace GLTech.Demos.SampleBlockMap
{
    // Um pequeno mapa usando BlockMap
    public class Map : Scene
    {
        TextureBuffer floors_buffer = Utils.GetImageFromBitmap(Resources.Floors);
        TextureBuffer golden_buffer = Utils.GetImageFromBitmap(Resources.golden);
        TextureBuffer got_buffer = Utils.GetImageFromBitmap(Resources.GOT);
        TextureBuffer background_buffer = Utils.GetImageFromBitmap(Resources.Universe);

        protected override void Delete()
        {
            floors_buffer.Dispose();
            golden_buffer.Dispose();
            got_buffer.Dispose();
            background_buffer.Dispose();
        }

        public Map()
        {
            Background = new Texture(background_buffer);

            // BlockMap
            {
                using TextureBuffer blockmap_buffer = Utils.GetImageFromBitmap(Resources.MapGrid);
                Dictionary<Color, Texture> dict = new();
                {
                    Texture floors = new Texture(
                        source: floors_buffer,
                        hrepeat: 1f,
                        vrepeat: 0.25f);

                    Texture golden = new Texture(
                        source: golden_buffer,
                        hrepeat: 4f);

                    Texture got = new Texture(
                        source: got_buffer,
                        hrepeat: 8f,
                        vrepeat: 2f);

                    dict[(255, 255, 255)] = floors;
                    dict[(0, 192, 0)] = golden;
                    dict[(128, 0, 255)] = got;
                }

                // BlockMap map = new BlockMap(map: grid, textureBindings: mapper);
                BlockMap blockMap = new(
                    map: blockmap_buffer,
                    mapTexture: color =>
                    {
                        if (dict.TryGetValue(color, out Texture texture))
                            return texture;
                        else return null;
                    },
                    textureFilling: BlockMap.TextureFilling.Block,
                    optimize: true,
                    colliders: true);

                Add(blockMap);
            }

            // Camera
            {
                Player.AddScript<DebugPerformance>();
                Player.RelativeRotation = -90f;

                // Tratador de colisão
                KinematicBody body = new();
                body.StartPosition = (8f, 8f);
                Q1Controller controller = new();

                Player.AddScript(controller);
                Player.AddScript(body);
            }
        }
    }
}
