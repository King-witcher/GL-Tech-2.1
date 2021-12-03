﻿using GLTech2;
using GLTech2.Imaging;
using GLTech2.Entities;
using GLTech2.Entities.StandardEntites;
using GLTech2.Scripting;
using GLTech2.Scripting.Debugging;
using GLTech2.Scripting.StandardScripts;
using GLTech2.Scripting.Physics;

using System.Threading.Tasks;

namespace Test
{
    partial class Program
    {
        static ImageData Resize(ImageData pb, int scale)
        {
            ImageData resized = new ImageData(pb.Width * scale, pb.Height * scale);

            Parallel.For(0, resized.Height, line =>
            {
                float float_match_line = (float)line / scale;

                int matchline = (int)float_match_line;
                float rest_line = float_match_line - matchline;

                for (int column = 0; column < resized.Width ; column++)
                {
                    float float_match_col = (float)column / scale;

                    int matchcol = (int)float_match_col;
                    float rest_col = float_match_col - matchcol;

                    Pixel top = pb[matchcol, matchline].Mix(pb[matchcol + 1, matchline], rest_col);
                    Pixel bot = pb[matchcol, matchline + 1].Mix(pb[matchcol + 1, matchline + 1], rest_col);

                    Pixel center = top.Mix(bot, rest_line);
                    resized[column, line] = center;
                }
            });

            return resized;
        }

        class EnableNoclip : Behaviour
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
        static void E1M1()
        {
            // Buffers used
            using ImageData textures = new ImageData(WE1M1.WolfTextures);
            //using PixelBuffer textures_ = Resize(textures_, 20);
            using ImageData background_buffer = new ImageData(WE1M1.Background);
            using ImageData lula_buffer = new ImageData(WE1M1.lula);
            using ImageData pt_buffer = new ImageData(WE1M1.pt);
            using ImageData bolsonaro_buffer = new ImageData(WE1M1.bolsonaro);
            using ImageData dolar_buffer = new ImageData(WE1M1._1dolar);

            Texture background = new Texture(background_buffer);
            using Scene scene = new Scene(background);

            // BlockMap
            {
                using ImageData grid = new ImageData(WE1M1.MapGrid);
                BlockMap.TextureMapper binds = new BlockMap.TextureMapper();
                {
                    Texture blueStone1 = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 2 / 19f);

                    Texture blueStone2 = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 2 / 19f);

                    Texture bluestoneCell = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 1 / 19f);

                    Texture bluestoneCellSkeleton = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 2 / 19f);

                    Texture grayStone1 = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 0 / 19f);

                    Texture grayStone2 = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 0 / 19f);

                    Texture gs_naziFlag = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 0 / 19f);

                    Texture gs_hitler = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 1 / 19f);

                    Texture gs_goldEagle = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 1 / 19f);

                    Texture woodPanelling = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 3 / 19f);

                    Texture wp_whiteEagle = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 3 / 19f);

                    Texture wp_hitler = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 3 / 19f);

                    Texture elevator = new Texture(
                        buffer: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 17 / 19f);

                    Texture pt = new Texture(pt_buffer);
                    Texture lula = new Texture(lula_buffer);
                    Texture bolsonaro = new Texture(bolsonaro_buffer);
                    Texture dolar = new Texture(dolar_buffer);

                    binds[(0, 0, 255)] = blueStone1;
                    binds[(0, 0, 128)] = blueStone2;
                    binds[(0, 0, 64)] = bluestoneCell;
                    binds[(0, 128, 0)] = bluestoneCellSkeleton;
                    binds[(128, 128, 128)] = grayStone1;
                    binds[(64, 64, 64)] = grayStone2;
                    binds[(255, 0, 0)] = gs_naziFlag;
                    binds[(128, 64, 0)] = gs_hitler;
                    binds[(255, 255, 0)] = gs_goldEagle;
                    binds[(255, 128, 0)] = woodPanelling;
                    binds[(255, 192, 128)] = wp_whiteEagle;
                    binds[(80, 40, 0)] = wp_hitler;
                    binds[(128, 255, 128)] = elevator;
                }

                BlockMap BlockMap = new BlockMap(map: grid, textureBindings: binds);
                scene.Add(BlockMap);
            }

            // Camera
            {
                Camera camera = scene.Camera;
                camera.WorldPosition = (57.5f, 29.5f);
                camera.AddBehaviour<DebugPerformanceStats>();
                camera.AddBehaviour<DebugSceneInfo>();
                camera.AddBehaviour(new MouseLook(2.2f));
                camera.AddBehaviour<ClickToMakeRotate>();

                PointCollider pc = new PointCollider();
                camera.AddBehaviour(pc);
                SoftMovement movement = new SoftMovement(pc);
                camera.AddBehaviour(movement);
                //camera.AddBehaviour<Rotate>();
            }

            // Renderer customization
            Engine.FullScreen = true;
            Engine.FieldOfView = 72f;
            Engine.ParallelRendering = true;
            Engine.DoubleBuffer = true;
            Engine.CaptureMouse = true;

            // Run!
            Engine.Run(scene);
        }
    }
}