using GLTech2.PrefabBehaviours;
using GLTech2.PrefabElements;

namespace GLTech2
{
    using GLTech2.Properties;
    partial class Program
    {
        static void AnimatedExample()
        {
            // Load buffers
            using (PixelBuffer background_buffer = (PixelBuffer)Resources.DoomSky)
            using (PixelBuffer carvedWall_buffer = (PixelBuffer)Resources.CarvedWall)
            using (PixelBuffer bricks_buffer = (PixelBuffer)Resources.Bricks)
            using (PixelBuffer wood_buffer = (PixelBuffer)Resources.Wood)
            using (PixelBuffer grayHexagons_buffer = (PixelBuffer)Resources.GrayHexagons)
            {
                // Scene
                Texture background = new Texture(
                    buffer: background_buffer,
                    hoffset: 0f,
                    hrepeat: 1f);

                Scene scene = new Scene(background);

                // Pivot
                {
                    Empty pivot = new Empty(0, 0.2868f);
                    pivot.AddBehaviour(new Rotate { Speed = -20f });

                    // Square
                    {
                        Texture tex = new Texture(
                            buffer: wood_buffer,
                            hoffset: 0f,
                            hrepeat: 2f);

                        Element e = new RegularPolygon((-0.5f, 0f), 4, .354f, tex);

                        e.AddBehaviour(new Rotate { Speed = 180f });
                        e.Parent = pivot;
                    }

                    // Cylinder
                    {
                        Texture tex = new Texture(
                            buffer: bricks_buffer,
                            hoffset: 0f,
                            hrepeat: 4f);

                        Element e = new RegularPolygon((0.5f, 0f), 100, .318f, tex);

                        e.AddBehaviour(new Rotate { Speed = 180f });
                        e.Parent = pivot;
                    }

                    // Triangle
                    {
                        Texture tex = new Texture(
                            buffer: carvedWall_buffer,
                            hoffset: 0f,
                            hrepeat: 1f);

                        Element e = new RegularPolygon((0f, 0.866f), 3, .385f, tex);

                        e.AddBehaviour(new Rotate { Speed = 180f });
                        e.Parent = pivot;
                    }

                    scene.AddElement(pivot);
                }

                // Big triangle
                {
                    Texture tex = new Texture(
                        buffer: grayHexagons_buffer,
                        hoffset: 0f,
                        hrepeat: 32f);

                    Element e = new RegularPolygon(Vector.Origin, 4, 2, tex);

                    scene.AddElement(e);
                }

                // Observer
                {
                    Observer pov = new Observer(Vector.Backward, 0);

                    pov.AddBehaviour<DebugPosition>();
                    pov.AddBehaviour<NoclipController>();
                    pov.AddBehaviour(new MouseLook(2.2f));

                    scene.AddElement(pov);
                }

                // Setup Renderer
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
