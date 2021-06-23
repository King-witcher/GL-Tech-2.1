using GLTech2.PrefabBehaviours;
using GLTech2.PrefabElements;
using System.Collections.Generic;

namespace GLTech2
{
    using GLTech2.Properties;
    partial class Program
    {
        static void GridExample()
        {
            using (PixelBuffer grid = new PixelBuffer(Resources.MapGrid))
            using (PixelBuffer bricks_buffer = new PixelBuffer(Resources.Bricks))
            using (PixelBuffer wood_buffer = new PixelBuffer(Resources.Wood))
            using (PixelBuffer hexagons_buffer = new PixelBuffer(Resources.GrayHexagons))
            using (PixelBuffer background_buffer = new PixelBuffer(Resources.DoomSky))
            {
                Texture background = new Texture(background_buffer);
                Scene scene = new Scene(background);

				// GridMap
				{
                    Dictionary<RGB, Texture> dict = new Dictionary<RGB, Texture>();
                    {
                        Texture bricks = new Texture(
                            buffer: bricks_buffer,
                            hrepeat: 2f);
                        Texture wood = new Texture(
                            buffer: wood_buffer,
                            hrepeat: 1f);
                        Texture hexagons = new Texture(
                            buffer: hexagons_buffer,
                            hrepeat: 2f);

                        dict[(255, 255, 255)] = bricks;
                        dict[(0, 192, 0)] = wood;
                        dict[(128, 0, 255)] = hexagons;
                    }

                    GridMap gridMap = new GridMap(grid, dict);
                    scene.AddElement(gridMap);
                }

                // Observer
                {
                    Observer pov = new Observer((5, 5), 180);

                    pov.AddBehaviour<DebugFPS>();
                    pov.AddBehaviour<DebugPosition>();
                    pov.AddBehaviour<DebugWallCount>();
                    pov.AddBehaviour<FlatMovement>();
                    pov.AddBehaviour(new MouseLook(2.2f));

                    scene.AddElement(pov);
                }

                // Renderer customization
                Renderer.FullScreen = true;
                Renderer.FieldOfView = 110f;
                Renderer.ParallelRendering = true;
                Renderer.DoubleBuffering = true;
                Renderer.CaptureMouse = true;

                // Run!
                Renderer.Run(scene);

                // Release scene
                scene.Dispose();
            }
        }
    }
}
