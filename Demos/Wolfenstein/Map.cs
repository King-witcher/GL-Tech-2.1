using Engine;
using Engine.Imaging;
using Engine.Input;
using Engine.World;
using Engine.World.Prefab;
using Engine.Scripting;
using Engine.Scripting.Debugging;
using Engine.Scripting.Physics;
using Engine.Scripting.Prefab;

using System.Threading.Tasks;
using System.Collections.Generic;

namespace Engine.Demos.Wolfenstein
{

    class EnableNoclip : Script
    {
        FlatMovement fm;

        public EnableNoclip(FlatMovement fm)
        {
            this.fm = fm;
        }

        void OnFrame()
        {
            if (Keyboard.IsKeyDown(InputKey.F5))
                fm.HandleCollisions = false;
            if (Keyboard.IsKeyDown(InputKey.F6))
                fm.HandleCollisions = true;
        }
    }

    // Essa é a priemira fase do Wolfenstein 3D
    public class Map : Scene
    {
        Image textures = new(Resources.WolfTextures);
        Image background_buffer = new(Resources.Background);
        Image lula_buffer = new(Resources.lula);
        Image pt_buffer = new(Resources.pt);
        Image bolsonaro_buffer = new(Resources.bolsonaro);
        Image dolar_buffer = new(Resources._1dolar);

        protected override void Delete()
        {
            textures.Dispose();
            background_buffer.Dispose();
            lula_buffer.Dispose();
            pt_buffer.Dispose();
            bolsonaro_buffer.Dispose();
            dolar_buffer.Dispose();
        }

        public Map()
        {
            Background = new Texture(background_buffer);

            // BlockMap
            {
                using Image blockmap_buffer = new Image(Resources.MapGrid);
                Dictionary<Color, Texture> dict = new();
                {
                    Texture blueStone1 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 2 / 19f);

                    Texture blueStone2 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 2 / 19f);

                    Texture bluestoneCell = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 1 / 19f);

                    Texture bluestoneCellSkeleton = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 2 / 19f);

                    Texture grayStone1 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 0 / 19f);

                    Texture grayStone2 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 0 / 19f);

                    Texture gs_naziFlag = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 0 / 19f);

                    Texture gs_hitler = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 1 / 19f);

                    Texture gs_goldEagle = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 1 / 19f);

                    Texture woodPanelling = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 3 / 19f);

                    Texture wp_whiteEagle = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 3 / 19f);

                    Texture wp_hitler = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 3 / 19f);

                    Texture elevator = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 17 / 19f);

                    Texture pt = new Texture(pt_buffer);
                    Texture lula = new Texture(lula_buffer);
                    Texture bolsonaro = new Texture(bolsonaro_buffer);
                    Texture dolar = new Texture(dolar_buffer);

                    dict[(0, 0, 255)] = blueStone1;
                    dict[(0, 0, 128)] = blueStone2;
                    dict[(0, 0, 64)] = bluestoneCell;
                    dict[(0, 128, 0)] = bluestoneCellSkeleton;
                    dict[(128, 128, 128)] = grayStone1;
                    dict[(64, 64, 64)] = grayStone2;
                    dict[(255, 0, 0)] = gs_naziFlag;
                    dict[(128, 64, 0)] = gs_hitler;
                    dict[(255, 255, 0)] = gs_goldEagle;
                    dict[(255, 128, 0)] = woodPanelling;
                    dict[(255, 192, 128)] = wp_whiteEagle;
                    dict[(80, 40, 0)] = wp_hitler;
                    dict[(128, 255, 128)] = elevator;
                }

                // BlockMap BlockMap = new BlockMap(map: grid, textureBindings: binds);

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
                    colliders: true);
                Add(blockMap);
            }

            // Camera
            {
                Camera camera = Camera;
                camera.WorldPosition = (57.5f, 29.5f);
                camera.AddScript<DebugPerformanceStats>();
                camera.AddScript<DebugSceneInfo>();
                camera.AddScript(new MouseLook(2.2f));

                PointCollider pc = new PointCollider();
                camera.AddScript(pc);
                SoftMovement movement = new SoftMovement(pc);
                camera.AddScript(movement);
                //camera.AddScript<Rotate>();
            }

            // Renderer customization
            Engine.Renderer.FullScreen = true;
            Engine.Renderer.FieldOfView = 72f;
            Engine.Renderer.ParallelRendering = true;
            Engine.Renderer.SynchronizeThreads = true;
            Engine.Renderer.CaptureMouse = true;
        }
        static Image Resize(Image pb, int scale)
        {
            Image resized = new Image(pb.Width * scale, pb.Height * scale);

            Parallel.For(0, resized.Height, line =>
            {
                float float_match_line = (float)line / scale;

                int matchline = (int)float_match_line;
                float rest_line = float_match_line - matchline;

                for (int column = 0; column < resized.Width; column++)
                {
                    float float_match_col = (float)column / scale;

                    int matchcol = (int)float_match_col;
                    float rest_col = float_match_col - matchcol;

                    Color top = pb[matchcol, matchline].Mix(pb[matchcol + 1, matchline], rest_col);
                    Color bot = pb[matchcol, matchline + 1].Mix(pb[matchcol + 1, matchline + 1], rest_col);

                    Color center = top.Mix(bot, rest_line);
                    resized[column, line] = center;
                }
            });

            return resized;
        }
    }
}