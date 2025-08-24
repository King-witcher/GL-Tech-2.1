using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Demos;

using System.Collections.Generic;

namespace Engine.Demos.SampleBlockMap
{
    // Um pequeno mapa usando BlockMap
    public class Map : Scene
    {
        Image floors_buffer = new(Resources.Floors);
        Image golden_buffer = new(Resources.golden);
        Image got_buffer = new(Resources.GOT);
        Image background_buffer = new(Resources.Universe);

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
                using Image blockmap_buffer = new Image(Resources.MapGrid);
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
                        else return Texture.NullTexture;
                    },
                    textureFilling: BlockMap.TextureFilling.Block,
                    optimize: true,
                    colliders: true);

                Add(blockMap);
            }

            // Camera
            {
                Camera.WorldPosition = (5f, 5f);
                Camera.RelativeRotation = -90f;

                Camera.AddScript<DebugPerformance>();
                Camera.AddScript<DebugScene>();
                Camera.AddScript<DebugEntity>();

                PointCollider collider = new PointCollider();
                SoftMovement movement = new SoftMovement(collider);
                MouseLook mouseLook = new MouseLook(2.2f);

                Camera.AddScript(collider);
                Camera.AddScript(movement);
                Camera.AddScript(mouseLook);
            }
        }
    }
}